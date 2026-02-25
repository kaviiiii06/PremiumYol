// PremiumYol JavaScript

// ========================================
// DARK/LIGHT MODE TOGGLE
// ========================================

document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('theme-toggle');
    const themeIcon = document.getElementById('theme-icon');
    const html = document.documentElement;
    
    // Tema zaten head'de yüklendi, sadece ikonu güncelle
    const currentTheme = html.getAttribute('data-theme') || 'light';
    updateThemeIcon(currentTheme);
    
    // Theme toggle click event
    if (themeToggle) {
        themeToggle.addEventListener('click', function(e) {
            e.preventDefault();
            const currentTheme = html.getAttribute('data-theme');
            const newTheme = currentTheme === 'light' ? 'dark' : 'light';
            
            // Change theme
            html.setAttribute('data-theme', newTheme);
            localStorage.setItem('theme', newTheme);
            updateThemeIcon(newTheme);
            
            // Add animation
            themeIcon.style.transform = 'rotate(360deg)';
            setTimeout(() => {
                themeIcon.style.transform = 'rotate(0deg)';
            }, 300);
        });
    }
    
    function updateThemeIcon(theme) {
        if (themeIcon) {
            if (theme === 'dark') {
                themeIcon.className = 'fas fa-sun';
                themeToggle.title = 'Aydınlık Tema';
            } else {
                themeIcon.className = 'fas fa-moon';
                themeToggle.title = 'Karanlık Tema';
            }
        }
    }
});

$(document).ready(function() {
    // Sayfa yüklendiğinde sepet sayısını güncelle
    updateCartCount();
    
    // Ana sayfadaki sepete ekle butonları
    $('.add-to-cart').click(function() {
        var productId = $(this).data('product-id');
        addToCart(productId);
    });
    
    // Arama barı animasyonu
    initSearchBar();
});

function updateCartCount() {
    $.get('/Cart/GetCartCount', function(data) {
        if ($('#cart-count').length > 0) {
            $('#cart-count').text(data.count || 0);
        }
    }).fail(function() {
        console.log('Sepet sayısı alınamadı');
    });
}

function addToCart(productId, quantity = 1) {
    $.post('/Cart/AddToCart', { productId: productId, quantity: quantity }, function(data) {
        if (data.success) {
            $('#cart-count').text(data.cartCount);
            showToast('Başarılı!', data.message, 'success');
        } else {
            showToast('Hata!', data.message, 'error');
        }
    }).fail(function() {
        showToast('Hata!', 'Bir hata oluştu. Lütfen tekrar deneyin.', 'error');
    });
}

function showToast(title, message, type = 'success') {
    // Mevcut toast'ları temizle
    $('.custom-toast').remove();
    
    var bgGradient = type === 'success' 
        ? 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
        : 'linear-gradient(135deg, #f5576c 0%, #f093fb 100%)';
    var iconClass = type === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle';
    
    var toast = $(`
        <div class="custom-toast position-fixed top-0 start-50 translate-middle-x mt-3 animate__animated animate__fadeInDown" 
             style="z-index: 9999; min-width: 350px; max-width: 500px;">
            <div class="card border-0 shadow-lg" style="background: ${bgGradient}; border-radius: 15px;">
                <div class="card-body p-3 text-white">
                    <div class="d-flex align-items-center">
                        <i class="${iconClass} fa-2x me-3"></i>
                        <div class="flex-grow-1">
                            <h6 class="mb-1 fw-bold">${title}</h6>
                            <p class="mb-0 small">${message}</p>
                        </div>
                        <button type="button" class="btn-close btn-close-white ms-2" onclick="$(this).closest('.custom-toast').remove()"></button>
                    </div>
                </div>
            </div>
        </div>
    `);
    
    $('body').append(toast);
    
    // 4 saniye sonra otomatik kaldır
    setTimeout(function() {
        toast.addClass('animate__fadeOutUp');
        setTimeout(function() {
            toast.remove();
        }, 500);
    }, 4000);
}

// Sayfa geçişlerinde loading göster
$(document).on('click', 'a, button[type="submit"]', function() {
    if (!$(this).hasClass('no-loading')) {
        showLoading();
    }
});

function showLoading() {
    if ($('#loading-overlay').length === 0) {
        var loadingOverlay = $(`
            <div id="loading-overlay" class="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" 
                 style="background-color: rgba(255,255,255,0.8); z-index: 9999;">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Yükleniyor...</span>
                </div>
            </div>
        `);
        $('body').append(loadingOverlay);
        
        // 5 saniye sonra otomatik kaldır
        setTimeout(function() {
            $('#loading-overlay').remove();
        }, 5000);
    }
}

$(window).on('load', function() {
    $('#loading-overlay').remove();
});

// Kampanya daireleri kaydırma
$('.circle-nav-prev').click(function() {
    var container = $('.campaign-circles');
    container.animate({
        scrollLeft: container.scrollLeft() - 300
    }, 300);
});

$('.circle-nav-next').click(function() {
    var container = $('.campaign-circles');
    container.animate({
        scrollLeft: container.scrollLeft() + 300
    }, 300);
});

// ========================================
// SEARCH BAR ENHANCEMENTS
// ========================================

function initSearchBar() {
    const searchInput = $('.search-input');
    const searchButton = $('.search-button');
    
    // Enter tuşu ile arama
    searchInput.on('keypress', function(e) {
        if (e.which === 13) {
            $(this).closest('form').submit();
        }
    });
    
    // Input değiştiğinde buton animasyonu
    searchInput.on('input', function() {
        if ($(this).val().length > 0) {
            searchButton.addClass('pulse');
        } else {
            searchButton.removeClass('pulse');
        }
    });
    
    // Focus animasyonu
    searchInput.on('focus', function() {
        $('.search-wrapper').addClass('focused');
    });
    
    searchInput.on('blur', function() {
        $('.search-wrapper').removeClass('focused');
    });
}

// Arama önerileri (gelecek özellik için hazır)
function showSearchSuggestions(query) {
    // TODO: API'den arama önerileri getir
}

// ========================================
// SEARCH AUTOCOMPLETE
// ========================================

$(document).ready(function() {
    const searchInput = $('#searchInput');
    const autocompleteDiv = $('#searchAutocomplete');
    let searchTimeout;

    if (searchInput.length > 0) {
        // Input değiştiğinde otomatik tamamlama
        searchInput.on('input', function() {
            const term = $(this).val().trim();
            
            clearTimeout(searchTimeout);
            
            if (term.length < 2) {
                autocompleteDiv.hide();
                return;
            }
            
            searchTimeout = setTimeout(function() {
                loadAutocomplete(term);
            }, 300);
        });

        // Focus olduğunda son aramalar veya popüler aramalar göster
        searchInput.on('focus', function() {
            const term = $(this).val().trim();
            if (term.length >= 2) {
                loadAutocomplete(term);
            } else {
                loadSearchHistory();
            }
        });

        // Dışarı tıklandığında kapat
        $(document).on('click', function(e) {
            if (!$(e.target).closest('.search-container').length) {
                autocompleteDiv.hide();
            }
        });

        // Klavye navigasyonu
        searchInput.on('keydown', function(e) {
            const items = autocompleteDiv.find('.autocomplete-item');
            const activeItem = items.filter('.active');
            
            if (e.key === 'ArrowDown') {
                e.preventDefault();
                if (activeItem.length === 0) {
                    items.first().addClass('active');
                } else {
                    activeItem.removeClass('active').next('.autocomplete-item').addClass('active');
                }
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                if (activeItem.length > 0) {
                    activeItem.removeClass('active').prev('.autocomplete-item').addClass('active');
                }
            } else if (e.key === 'Enter') {
                if (activeItem.length > 0) {
                    e.preventDefault();
                    activeItem.click();
                }
            } else if (e.key === 'Escape') {
                autocompleteDiv.hide();
            }
        });
    }
});

function loadAutocomplete(term) {
    $.get('/Search/Autocomplete', { term: term }, function(data) {
        displayAutocomplete(data);
    });
}

function loadSearchHistory() {
    $.get('/Search/History', function(data) {
        if (data && data.length > 0) {
            displaySearchHistory(data);
        } else {
            loadPopularSearches();
        }
    });
}

function loadPopularSearches() {
    $.get('/Search/Popular', function(data) {
        if (data && data.length > 0) {
            displayPopularSearches(data);
        }
    });
}

function displayAutocomplete(results) {
    const autocompleteDiv = $('#searchAutocomplete');
    
    if (!results || results.length === 0) {
        autocompleteDiv.hide();
        return;
    }
    
    let html = '<div class="autocomplete-list">';
    
    results.forEach(function(item) {
        let icon = 'fa-search';
        let badge = '';
        
        if (item.tip === 'Ürün') {
            icon = 'fa-box';
            badge = '<span class="badge bg-primary ms-2">Ürün</span>';
        } else if (item.tip === 'Kategori') {
            icon = 'fa-th-large';
            badge = '<span class="badge bg-success ms-2">Kategori</span>';
        } else if (item.tip === 'Marka') {
            icon = 'fa-tag';
            badge = '<span class="badge bg-warning ms-2">Marka</span>';
        } else if (item.tip === 'Geçmiş') {
            icon = 'fa-history';
            badge = '<span class="badge bg-secondary ms-2">Geçmiş</span>';
        }
        
        html += `
            <div class="autocomplete-item" data-term="${item.terim}">
                <i class="fas ${icon} me-2 text-muted"></i>
                <span class="flex-grow-1">${item.terim}</span>
                ${badge}
            </div>
        `;
    });
    
    html += '</div>';
    
    autocompleteDiv.html(html).show();
    
    // Tıklama olayı
    $('.autocomplete-item').on('click', function() {
        const term = $(this).data('term');
        $('#searchInput').val(term);
        $('#searchInput').closest('form').submit();
    });
}

function displaySearchHistory(history) {
    const autocompleteDiv = $('#searchAutocomplete');
    
    let html = '<div class="autocomplete-list">';
    html += '<div class="autocomplete-header">Son Aramalarınız</div>';
    
    history.forEach(function(term) {
        html += `
            <div class="autocomplete-item" data-term="${term}">
                <i class="fas fa-history me-2 text-muted"></i>
                <span class="flex-grow-1">${term}</span>
                <button class="btn btn-sm btn-link text-muted remove-history" data-term="${term}">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;
    });
    
    html += `
        <div class="autocomplete-footer">
            <button class="btn btn-sm btn-link text-danger" id="clearHistory">
                <i class="fas fa-trash me-1"></i>Geçmişi Temizle
            </button>
        </div>
    `;
    html += '</div>';
    
    autocompleteDiv.html(html).show();
    
    // Tıklama olayları
    $('.autocomplete-item').on('click', function(e) {
        if (!$(e.target).closest('.remove-history').length) {
            const term = $(this).data('term');
            $('#searchInput').val(term);
            $('#searchInput').closest('form').submit();
        }
    });
    
    $('#clearHistory').on('click', function(e) {
        e.preventDefault();
        $.post('/Search/ClearHistory', function(data) {
            if (data.success) {
                autocompleteDiv.hide();
                showToast('Başarılı', 'Arama geçmişi temizlendi', 'success');
            }
        });
    });
}

function displayPopularSearches(searches) {
    const autocompleteDiv = $('#searchAutocomplete');
    
    let html = '<div class="autocomplete-list">';
    html += '<div class="autocomplete-header">Popüler Aramalar</div>';
    
    searches.forEach(function(term) {
        html += `
            <div class="autocomplete-item" data-term="${term}">
                <i class="fas fa-fire me-2 text-danger"></i>
                <span class="flex-grow-1">${term}</span>
            </div>
        `;
    });
    
    html += '</div>';
    
    autocompleteDiv.html(html).show();
    
    // Tıklama olayı
    $('.autocomplete-item').on('click', function() {
        const term = $(this).data('term');
        $('#searchInput').val(term);
        $('#searchInput').closest('form').submit();
    });
}
