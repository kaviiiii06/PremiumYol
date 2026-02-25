document.addEventListener('DOMContentLoaded', function() {
    var maxStock = parseInt(document.getElementById('maxStock')?.value || '0');
    
    // Miktar artır
    var increaseBtn = document.getElementById('increaseQty');
    if (increaseBtn) {
        increaseBtn.addEventListener('click', function() {
            var qtyInput = document.getElementById('quantity');
            var qty = parseInt(qtyInput.value);
            if (qty < maxStock) {
                qtyInput.value = qty + 1;
            }
        });
    }
    
    // Miktar azalt
    var decreaseBtn = document.getElementById('decreaseQty');
    if (decreaseBtn) {
        decreaseBtn.addEventListener('click', function() {
            var qtyInput = document.getElementById('quantity');
            var qty = parseInt(qtyInput.value);
            if (qty > 1) {
                qtyInput.value = qty - 1;
            }
        });
    }
    
    // Sepete ekle
    var addToCartBtn = document.getElementById('addToCartBtn');
    if (addToCartBtn) {
        addToCartBtn.addEventListener('click', function() {
            var productId = this.getAttribute('data-product-id');
            var quantity = parseInt(document.getElementById('quantity').value);
            var addToCartUrl = document.getElementById('addToCartUrl').value;
            
            fetch(addToCartUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: 'productId=' + productId + '&quantity=' + quantity
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    var cartCount = document.getElementById('cart-count');
                    if (cartCount) {
                        cartCount.textContent = data.cartCount;
                    }
                    
                    var cartUrl = document.getElementById('cartUrl').value;
                    
                    // Başarı mesajı
                    var toastHtml = '<div class="toast position-fixed top-0 end-0 m-3" role="alert">' +
                        '<div class="toast-header bg-success text-white">' +
                        '<i class="fas fa-check-circle me-2"></i>' +
                        '<strong class="me-auto">Başarılı!</strong>' +
                        '<button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>' +
                        '</div>' +
                        '<div class="toast-body">' + data.message + 
                        '<div class="mt-2">' +
                        '<a href="' + cartUrl + '" class="btn btn-sm btn-primary">Sepete Git</a>' +
                        '</div>' +
                        '</div>' +
                        '</div>';
                    
                    var tempDiv = document.createElement('div');
                    tempDiv.innerHTML = toastHtml;
                    var toastElement = tempDiv.firstChild;
                    document.body.appendChild(toastElement);
                    
                    var bsToast = new bootstrap.Toast(toastElement, { delay: 5000 });
                    bsToast.show();
                    
                    toastElement.addEventListener('hidden.bs.toast', function() {
                        this.remove();
                    });
                } else {
                    alert(data.message);
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Bir hata oluştu!');
            });
        });
    }

    // Yıldız rating sistemi
    var stars = document.querySelectorAll('.star-rating .star');
    var selectedRating = document.getElementById('selectedRating');
    var ratingDisplay = document.getElementById('ratingDisplay');
    
    var ratingTexts = {
        1: "1 Yıldız - Çok Kötü",
        2: "2 Yıldız - Kötü", 
        3: "3 Yıldız - Orta",
        4: "4 Yıldız - İyi",
        5: "5 Yıldız - Mükemmel"
    };
    
    stars.forEach(function(star, index) {
        star.addEventListener('click', function() {
            var rating = this.getAttribute('data-rating');
            selectedRating.value = rating;
            
            // Tüm yıldızları temizle
            stars.forEach(function(s) {
                s.classList.remove('active');
            });
            
            // Seçilen yıldıza kadar olanları aktif yap
            for (var i = 0; i < rating; i++) {
                stars[i].classList.add('active');
            }
            
            // Seçilen puanı göster
            ratingDisplay.textContent = ratingTexts[rating];
            ratingDisplay.style.color = '#ffc107';
            ratingDisplay.style.fontWeight = 'bold';
        });
        
        star.addEventListener('mouseenter', function() {
            var rating = this.getAttribute('data-rating');
            
            // Hover efekti
            stars.forEach(function(s) {
                s.classList.remove('hover');
            });
            
            for (var i = 0; i < rating; i++) {
                stars[i].classList.add('hover');
            }
            
            // Hover sırasında puanı göster
            ratingDisplay.textContent = ratingTexts[rating];
            ratingDisplay.style.color = '#ffc107';
        });
        
        star.addEventListener('mouseleave', function() {
            // Hover efektini temizle
            stars.forEach(function(s) {
                s.classList.remove('hover');
            });
            
            // Seçili puan varsa onu göster, yoksa varsayılan metni
            if (selectedRating.value) {
                ratingDisplay.textContent = ratingTexts[selectedRating.value];
                ratingDisplay.style.color = '#ffc107';
                ratingDisplay.style.fontWeight = 'bold';
            } else {
                ratingDisplay.textContent = 'Puan seçiniz';
                ratingDisplay.style.color = '#666';
                ratingDisplay.style.fontWeight = 'normal';
            }
        });
    });

});