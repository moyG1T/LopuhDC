﻿using LopuhDC.Data.Remote.Models;
using LopuhDC.Domain.Commands;
using LopuhDC.Domain.Contexts;
using LopuhDC.Domain.Models;
using LopuhDC.Domain.Services;
using LopuhDC.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LopuhDC.ViewModels
{
    public class ProductsViewModel : ViewModel
    {
        private readonly INavService _navService;
        private readonly ProductContext _context;
        private readonly LopuhDbEntities _db;

        private readonly int _totalPageCount = 0;
        private int _pageCount = 0;
        private readonly int _pageSize = 20;
        private bool _lastPageLoaded = false;

        private SelectableInt _currentIndex = new SelectableInt(1);

        private readonly List<Product> _allUnsortedProducts = new List<Product>();
        private List<Product> _sortedProducts = new List<Product>();

        private List<SelectableInt> pageNumbers = new List<SelectableInt>();
        private List<Product> _products = new List<Product>();

        private string _searchText;
        private int _alphabeticFilter;
        private ProductType _selectedType;
        private readonly List<ProductType> _productTypes;

        public List<Product> Products
        {
            get => _products;
            set
            {
                _products = value; OnPropertyChanged();
            }
        }
        public List<SelectableInt> PageNumbers
        {
            get => pageNumbers; set
            {
                pageNumbers = value; OnPropertyChanged();
            }
        }

        public bool CanSwipeLeft { get; private set; }
        public bool CanSwipeRight { get; private set; }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilters(); }
        }
        public int AlphabeticFilter
        {
            get => _alphabeticFilter;
            set { _alphabeticFilter = value; ApplyFilters(); }
        }
        public ProductType SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; ApplyFilters(); OnPropertyChanged(); }
        }
        public List<ProductType> ProductTypes => _productTypes;

        public ICommand SelectPageCommand { get; }
        public ICommand SwipeLeftCommand { get; }
        public ICommand SwipeRightCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand RemoveProductCommand { get; }
        public ICommand AddNewProductCommand { get; }
        public ICommand ResetTypeFilterCommand { get; }

        public ProductsViewModel(INavService navService, ProductContext context, LopuhDbEntities db)
        {
            SelectPageCommand = new RelayCommand(ShowCurrentPage);
            SwipeLeftCommand = new RelayCommand(SwipeLeft);
            SwipeRightCommand = new RelayCommand(SwipeRight);
            EditProductCommand = new RelayCommand(EditProduct);
            AddNewProductCommand = new PushCommand(navService);
            RemoveProductCommand = new RelayCommand(RemoveProduct);
            ResetTypeFilterCommand = new RelayCommand(ResetTypeFilter);

            _navService = navService;
            _context = context;
            _db = db;

            _context.ProductAdded += OnProductAdded;

            _productTypes = _db.ProductTypes.ToList();

            _totalPageCount = (int)Math.Ceiling((double)_db.Products.Count() / _pageSize);
            _pageCount = _totalPageCount;

            PageNumbers = Enumerable.Range(1, _pageCount).Select(it => new SelectableInt(it)).ToList();

            if (_pageCount > 0)
            {
                _allUnsortedProducts = _db.Products
                    .OrderBy(it => it.Title)
                    .Take(_pageSize)
                    .ToList();

                _currentIndex = PageNumbers.First();
                _currentIndex.IsSelected = true;
                _currentIndex.Loaded = true;

                CanSwipeRight = true;

                Products = _allUnsortedProducts;
            }
            else
            {
                _allUnsortedProducts = _db.Products.ToList();

                Products = _allUnsortedProducts;
            }
        }

        private void ShowCurrentPage(object param)
        {
            if (param is SelectableInt selectableInt)
            {
                var offset = selectableInt.Index;

                //if (offset == _currentIndex.Index)
                //    return;
                if (offset != _currentIndex.Index)
                {
                    _currentIndex.IsSelected = false;
                    _currentIndex = selectableInt;
                    _currentIndex.IsSelected = true;
                }

                CheckSwipes();

                offset--;

                var products = _sortedProducts.Count > 0 ? _sortedProducts : _allUnsortedProducts;

                var page = AlphabeticFilter == 0
                    ? products
                        .OrderBy(it => it.Title)
                        .Skip(_pageSize * offset)
                        .Take(_pageSize)
                        .ToList()
                    : products
                        .OrderByDescending(it => it.Title)
                        .Skip(_pageSize * offset)
                        .Take(_pageSize)
                        .ToList();

                if ((page.Count < _pageCount || !_currentIndex.Loaded) && _sortedProducts.Count == 0)
                {
                    try
                    {
                        var lastPage = PageNumbers.Last().Index == selectableInt.Index;

                        if (!(lastPage && _lastPageLoaded))
                        {
                            page = LoadMoreFromRemote(offset);
                            _currentIndex.Loaded = true;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                Products = page;
            }
        }
        private List<Product> LoadMoreFromRemote(int offset)
        {
            var products = AlphabeticFilter == 0
                    ? _db.Products
                        .OrderBy(it => it.Title)
                        .Skip(_pageSize * offset)
                        .Take(_pageSize)
                        .ToList()
                    : _db.Products
                        .OrderByDescending(it => it.Title)
                        .Skip(_pageSize * offset)
                        .Take(_pageSize)
                        .ToList();

            _allUnsortedProducts.AddRange(products);

            return products;
        }

        private void SwipeLeft()
        {
            var couldSwipeLeft = CanSwipeLeft;
            CanSwipeLeft = CheckCanSwipeLeft(out int currentIndex);
            if (!CanSwipeLeft)
            {
                return;
            }
            if (couldSwipeLeft != CanSwipeLeft)
            {
                OnPropertyChanged(nameof(CanSwipeLeft));
            }

            var leftItem = PageNumbers[currentIndex - 1];

            ShowCurrentPage(leftItem);
        }
        private void SwipeRight()
        {
            var couldSwipeRight = CanSwipeRight;
            CanSwipeRight = CheckCanSwipeRight(out int currentIndex);
            if (!CanSwipeRight)
            {
                return;
            }
            if (couldSwipeRight != CanSwipeRight)
            {
                OnPropertyChanged(nameof(CanSwipeRight));
            }

            var rightItem = PageNumbers[currentIndex + 1];

            ShowCurrentPage(rightItem);
        }

        private void CheckSwipes()
        {
            var couldSwipeLeft = CanSwipeLeft;
            var couldSwipeRight = CanSwipeRight;
            CanSwipeLeft = CheckCanSwipeLeft(out int _);
            CanSwipeRight = CheckCanSwipeRight(out int _);

            if (couldSwipeLeft != CanSwipeLeft)
            {
                OnPropertyChanged(nameof(CanSwipeLeft));
            }
            if (couldSwipeRight != CanSwipeRight)
            {
                OnPropertyChanged(nameof(CanSwipeRight));
            }
        }
        private bool CheckCanSwipeLeft(out int currentIndex)
        {
            var currentItem = PageNumbers.FirstOrDefault(it => it.IsSelected);

            if (currentItem == null)
            {
                currentIndex = -1;
                return false;
            }

            currentIndex = PageNumbers.IndexOf(currentItem);

            return currentIndex != 0;
        }
        private bool CheckCanSwipeRight(out int currentIndex)
        {
            var currentItem = PageNumbers.FirstOrDefault(it => it.IsSelected);

            if (currentItem == null)
            {
                currentIndex = -1;
                return false;
            }

            currentIndex = PageNumbers.IndexOf(currentItem);

            return currentIndex != PageNumbers.Count - 1;
        }

        private void ApplyFilters()
        {
            if (string.IsNullOrEmpty(SearchText) && SelectedType == null)
            {
                _sortedProducts.Clear();
                PageNumbers = Enumerable.Range(1, _totalPageCount).Select(it => new SelectableInt(it)).ToList();

                if (_totalPageCount > 0)
                {
                    _currentIndex = PageNumbers.First();
                    _currentIndex.IsSelected = true;
                    _currentIndex.Loaded = true;

                    var products = _sortedProducts.Count > 0 ? _sortedProducts : _allUnsortedProducts;

                    Products = AlphabeticFilter == 0
                        ? products
                            .OrderBy(it => it.Title)
                            .Take(_pageSize)
                            .ToList()
                        : products
                            .OrderByDescending(it => it.Title)
                            .Take(_pageSize)
                            .ToList();
                }
                else
                {
                    Products = AlphabeticFilter == 0
                        ? _allUnsortedProducts.OrderBy(it => it.Title).ToList()
                        : _allUnsortedProducts.OrderByDescending(it => it.Title).ToList();
                }
                CheckSwipes();
            }
            else if (string.IsNullOrEmpty(SearchText) && SelectedType != null)
            {
                _sortedProducts = _db.Products
                    .Where(it => it.TypeId == SelectedType.Id)
                    .ToList();

                _pageCount = (int)Math.Ceiling((double)_sortedProducts.Count / _pageSize);

                PageNumbers = Enumerable.Range(1, _pageCount).Select(it => new SelectableInt(it)).ToList();

                if (_pageCount > 0)
                {
                    _currentIndex = PageNumbers.First();
                    _currentIndex.IsSelected = true;
                    _currentIndex.Loaded = true;

                    Products = AlphabeticFilter == 0
                        ? _sortedProducts
                            .OrderBy(it => it.Title)
                            .Take(_pageSize)
                            .ToList()
                        : _sortedProducts
                            .OrderByDescending(it => it.Title)
                            .Take(_pageSize)
                            .ToList();
                }
                else
                {
                    Products = AlphabeticFilter == 0
                        ? _sortedProducts.OrderBy(it => it.Title).ToList()
                        : _sortedProducts.OrderByDescending(it => it.Title).ToList();
                }
                CheckSwipes();
            }
            else
            {
                _sortedProducts = SelectedType == null
                    ? _db.Products
                        .Where(it => it.Title.ToLower().Contains(SearchText.ToLower()))
                        .ToList()
                    : _db.Products
                        .Where(it => it.Title.ToLower().Contains(SearchText.ToLower())
                            && it.TypeId == SelectedType.Id)
                        .ToList();

                _pageCount = (int)Math.Ceiling((double)_sortedProducts.Count / _pageSize);

                PageNumbers = Enumerable.Range(1, _pageCount).Select(it => new SelectableInt(it)).ToList();

                if (_pageCount > 0)
                {
                    _currentIndex = PageNumbers.First();
                    _currentIndex.IsSelected = true;
                    _currentIndex.Loaded = true;

                    Products = AlphabeticFilter == 0
                        ? _sortedProducts
                            .OrderBy(it => it.Title)
                            .Take(_pageSize)
                            .ToList()
                        : _sortedProducts
                            .OrderByDescending(it => it.Title)
                            .Take(_pageSize)
                            .ToList();
                }
                else
                {
                    Products = AlphabeticFilter == 0
                        ? _sortedProducts.OrderBy(it => it.Title).ToList()
                        : _sortedProducts.OrderByDescending(it => it.Title).ToList();
                }
                CheckSwipes();
            }

            ResetPageLoadedProperty();
            _lastPageLoaded = false;
        }
        private void ResetTypeFilter()
        {
            SelectedType = null;
        }

        private void EditProduct(object param)
        {
            if (param is Product product)
            {
                _context.Product = product;
                _navService.Push();
            }
        }

        private void OnProductAdded(Product product)
        {
            if (_sortedProducts.Count > 0)
            {
                _sortedProducts.Add(product);
            }

            _allUnsortedProducts.Add(product);

            ResetPageLoadedProperty();
            ShowCurrentPage(_currentIndex);
        }
        private void RemoveProduct(object param)
        {
            if (param is Product product)
            {
                var result = MessageBox.Show("Удалить?", "Внимание", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
                {
                    return;
                }

                if (_sortedProducts.Contains(product))
                {
                    _sortedProducts.Remove(product);
                }

                _allUnsortedProducts.Remove(product);

                DeleteProduct(product);

                ResetPageLoadedProperty();
                ShowCurrentPage(_currentIndex);
            }
        }
        private void DeleteProduct(Product product)
        {
            if (product.ProductMaterials.Count > 0)
            {
                _db.ProductMaterials.RemoveRange(product.ProductMaterials);
                _db.SaveChanges();
            }

            _db.Products.Remove(product);
            _db.SaveChanges();
        }

        private void ResetPageLoadedProperty()
        {
            PageNumbers.ForEach(it => it.Loaded = false);
        }

        public override void Dispose()
        {
            _context.ProductAdded -= OnProductAdded;

            GC.SuppressFinalize(this);
        }
    }
}
