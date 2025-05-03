document.addEventListener("DOMContentLoaded", () => {
    // Initialize scroll animations
    initScrollAnimations()
  
    // Initialize back to top button
    initBackToTop()
  
    // Initialize product quick view
    initQuickView()
  
    // Initialize cookie consent
    initCookieConsent()
  
    // Initialize newsletter popup
    initNewsletterPopup()
  
    // Initialize product image zoom
    initProductZoom()
  
    // Initialize animated counters
    initCounters()
  
    // Initialize hover animations
    initHoverAnimations()
  
    // Initialize toast notifications system
    initToastSystem()
  
    // Scroll Animations
    function initScrollAnimations() {
      const revealElements = document.querySelectorAll(".reveal, .reveal-left, .reveal-right, .reveal-up")
  
      function checkReveal() {
        const windowHeight = window.innerHeight
        const revealPoint = 150
  
        revealElements.forEach((element) => {
          const elementTop = element.getBoundingClientRect().top
  
          if (elementTop < windowHeight - revealPoint) {
            element.classList.add("active")
          } else {
            // Optional: remove the class if you want the animation to replay when scrolling back up
            element.classList.remove('active');
          }
        })
      }
  
      // Initial check
      checkReveal()
  
      // Check on scroll
      window.addEventListener("scroll", checkReveal)
    }
  
    // Back to Top Button
    function initBackToTop() {
      const backToTopButton = document.createElement("button")
      backToTopButton.className = "back-to-top"
      backToTopButton.innerHTML = '<i class="fas fa-arrow-up"></i>'
      document.body.appendChild(backToTopButton)
  
      function toggleBackToTopButton() {
        if (window.pageYOffset > 300) {
          backToTopButton.classList.add("active")
        } else {
          backToTopButton.classList.remove("active")
        }
      }
  
      // Initial check
      toggleBackToTopButton()
  
      // Check on scroll
      window.addEventListener("scroll", toggleBackToTopButton)
  
      // Scroll to top on click
      backToTopButton.addEventListener("click", () => {
        window.scrollTo({
          top: 0,
          behavior: "smooth",
        })
      })
    }
  
    // Product Quick View
    function initQuickView() {
      // Create modal HTML
      const modalHTML = `
              <div class="modal fade quick-view-modal" id="quickViewModal" tabindex="-1" aria-hidden="true">
                  <div class="modal-dialog modal-lg modal-dialog-centered">
                      <div class="modal-content">
                          <div class="modal-header">
                              <h5 class="modal-title">Quick View</h5>
                              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                          </div>
                          <div class="modal-body">
                              <div class="quick-view-product">
                                  <div class="quick-view-image">
                                      <img src="/placeholder.svg" alt="Product Image" id="quickViewImage">
                                  </div>
                                  <div class="quick-view-info">
                                      <h2 class="quick-view-title" id="quickViewTitle"></h2>
                                      <p class="quick-view-price" id="quickViewPrice"></p>
                                      <div class="product-rating" id="quickViewRating"></div>
                                      <p class="quick-view-description" id="quickViewDescription"></p>
                                      <div class="quick-view-actions">
                                          <button class="btn btn-primary add-to-cart-quick" id="quickViewAddToCart">
                                              <i class="fas fa-shopping-cart me-2"></i>Add to Cart
                                          </button>
                                          <button class="btn btn-outline-secondary favorite-btn-quick" id="quickViewFavorite">
                                              <i class="far fa-heart"></i>
                                          </button>
                                      </div>
                                  </div>
                              </div>
                          </div>
                          <div class="modal-footer">
                              <a href="#" class="btn btn-link" id="quickViewFullDetails">View Full Details</a>
                          </div>
                      </div>
                  </div>
              </div>
          `
  
      // Append modal to body
      document.body.insertAdjacentHTML("beforeend", modalHTML)
  
      // Get modal elements
      const quickViewModal = document.getElementById("quickViewModal")
      const quickViewImage = document.getElementById("quickViewImage")
      const quickViewTitle = document.getElementById("quickViewTitle")
      const quickViewPrice = document.getElementById("quickViewPrice")
      const quickViewRating = document.getElementById("quickViewRating")
      const quickViewDescription = document.getElementById("quickViewDescription")
      const quickViewAddToCart = document.getElementById("quickViewAddToCart")
      const quickViewFavorite = document.getElementById("quickViewFavorite")
      const quickViewFullDetails = document.getElementById("quickViewFullDetails")
  
      // Sample product data (in a real app, this would come from a database)
      const products = [
        {
          id: 1,
          name: "Crystal Pendant Necklace",
          price: 299.99,
          originalPrice: null,
          description:
            "Elegant crystal pendant necklace with a gold chain. Perfect for special occasions or everyday wear.",
          category: "necklaces",
          image: "/placeholder.svg?height=400&width=300",
          rating: 4.5,
        },
        {
          id: 2,
          name: "Pearl Drop Earrings",
          price: 199.99,
          originalPrice: 249.99,
          description:
            "Beautiful pearl drop earrings with silver hooks. These classic earrings add elegance to any outfit.",
          category: "earrings",
          image: "/placeholder.svg?height=400&width=300",
          rating: 5,
        },
        {
          id: 3,
          name: "Gold Chain Bracelet",
          price: 249.99,
          originalPrice: null,
          description: "Stunning gold chain bracelet with a secure clasp. A timeless piece that complements any style.",
          category: "bracelets",
          image: "/placeholder.svg?height=400&width=300",
          rating: 4,
        },
      ]
  
      // Add quick view buttons to product cards
      const productCards = document.querySelectorAll(".product-card")
  
      productCards.forEach((card) => {
        // Get product ID from favorite button
        const favoriteBtn = card.querySelector(".favorite-btn")
        if (!favoriteBtn) return
  
        const productId = favoriteBtn.getAttribute("data-product-id")
  
        // Create quick view button
        const quickViewBtn = document.createElement("button")
        quickViewBtn.className = "btn btn-outline-secondary quick-view-btn"
        quickViewBtn.innerHTML = '<i class="fas fa-eye"></i> Quick View'
        quickViewBtn.setAttribute("data-product-id", productId)
  
        // Add button to product actions
        const productActions = card.querySelector(".product-actions")
        if (productActions) {
          productActions.appendChild(quickViewBtn)
        }
  
        // Add click event
        quickViewBtn.addEventListener("click", (e) => {
          e.preventDefault()
          openQuickView(productId)
        })
      })
  
      // Open quick view modal
      function openQuickView(productId) {
        // Find product by ID
        const product = products.find((p) => p.id == productId)
  
        if (!product) return
  
        // Update modal content
        quickViewImage.src = product.image
        quickViewImage.alt = product.name
        quickViewTitle.textContent = product.name
  
        // Format price
        if (product.originalPrice) {
          quickViewPrice.innerHTML = `
                      <span class="original-price">EGP ${product.originalPrice.toFixed(2)}</span>
                      EGP ${product.price.toFixed(2)}
                  `
        } else {
          quickViewPrice.textContent = `EGP ${product.price.toFixed(2)}`
        }
  
        // Render rating stars
        quickViewRating.innerHTML = renderStarRating(product.rating)
  
        // Set description
        quickViewDescription.textContent = product.description
  
        // Set product ID for add to cart and favorite buttons
        quickViewAddToCart.setAttribute("data-product-id", product.id)
        quickViewFavorite.setAttribute("data-product-id", product.id)
  
        // Check if product is in favorites
        const favorites = JSON.parse(localStorage.getItem("favorites")) || []
        if (favorites.includes(Number(product.id))) {
          quickViewFavorite.classList.add("active")
          quickViewFavorite.querySelector("i").classList.replace("far", "fas")
        } else {
          quickViewFavorite.classList.remove("active")
          quickViewFavorite.querySelector("i").classList.replace("fas", "far")
        }
  
        // Set full details link
        quickViewFullDetails.href = `product-detail.html?id=${product.id}`
  
        // Open modal
        const modal = new bootstrap.Modal(quickViewModal)
        modal.show()
      }
  
      // Add to cart from quick view
      quickViewAddToCart.addEventListener("click", function () {
        const productId = Number(this.getAttribute("data-product-id"))
        addToCart(productId)
        showToast("Product added to cart!", "success")
      })
  
      // Toggle favorite from quick view
      quickViewFavorite.addEventListener("click", function () {
        const productId = Number(this.getAttribute("data-product-id"))
        toggleFavorite(productId, this)
      })
  
      // Render star rating
      function renderStarRating(rating) {
        const fullStars = Math.floor(rating)
        const halfStar = rating % 1 >= 0.5
        const emptyStars = 5 - fullStars - (halfStar ? 1 : 0)
  
        let starsHTML = ""
  
        // Full stars
        for (let i = 0; i < fullStars; i++) {
          starsHTML += '<i class="fas fa-star"></i>'
        }
  
        // Half star
        if (halfStar) {
          starsHTML += '<i class="fas fa-star-half-alt"></i>'
        }
  
        // Empty stars
        for (let i = 0; i < emptyStars; i++) {
          starsHTML += '<i class="far fa-star"></i>'
        }
  
        return starsHTML
      }
  
      // Add to cart function
      function addToCart(productId) {
        const cart = JSON.parse(localStorage.getItem("cart")) || []
        const product = products.find((p) => p.id === productId)
  
        if (!product) return
  
        // Check if product is already in cart
        const existingItem = cart.find((item) => item.id === productId)
  
        if (existingItem) {
          // Increment quantity
          existingItem.quantity += 1
        } else {
          // Add new item
          cart.push({
            id: product.id,
            name: product.name,
            price: product.price,
            image: product.image,
            quantity: 1,
          })
        }
  
        // Save to localStorage
        localStorage.setItem("cart", JSON.stringify(cart))
  
        // Update cart count badge
        updateCountBadges()
      }
  
      // Toggle favorite function
      function toggleFavorite(productId, button) {
        const favorites = JSON.parse(localStorage.getItem("favorites")) || []
        const index = favorites.indexOf(productId)
  
        if (index === -1) {
          // Add to favorites
          favorites.push(productId)
          button.classList.add("active")
          button.querySelector("i").classList.replace("far", "fas")
          showToast("Product added to favorites!", "success")
        } else {
          // Remove from favorites
          favorites.splice(index, 1)
          button.classList.remove("active")
          button.querySelector("i").classList.replace("fas", "far")
          showToast("Product removed from favorites!", "info")
        }
  
        // Save to localStorage
        localStorage.setItem("favorites", JSON.stringify(favorites))
  
        // Update favorites count badge
        updateCountBadges()
      }
  
      // Update count badges
      function updateCountBadges() {
        const favorites = JSON.parse(localStorage.getItem("favorites")) || []
        const cart = JSON.parse(localStorage.getItem("cart")) || []
  
        const favCount = document.getElementById("favCount")
        const cartCount = document.getElementById("cartCount")
  
        if (favCount) favCount.textContent = favorites.length
        if (cartCount) cartCount.textContent = cart.length
      }
    }
  
    // Cookie Consent
    function initCookieConsent() {
      // Check if user has already consented
      if (localStorage.getItem("cookieConsent")) {
        return
      }
  
      // Create cookie consent HTML
      const cookieConsentHTML = `
              <div class="cookie-consent">
                  <div class="cookie-text">
                      <h4>We use cookies</h4>
                      <p>This website uses cookies to ensure you get the best experience on our website.</p>
                  </div>
                  <div class="cookie-buttons">
                      <button class="btn btn-outline-secondary" id="cookieDecline">Decline</button>
                      <button class="btn btn-primary" id="cookieAccept">Accept All</button>
                  </div>
              </div>
          `
  
      // Append to body
      document.body.insertAdjacentHTML("beforeend", cookieConsentHTML)
  
      // Get elements
      const cookieConsent = document.querySelector(".cookie-consent")
      const cookieAccept = document.getElementById("cookieAccept")
      const cookieDecline = document.getElementById("cookieDecline")
  
      // Show cookie consent after a delay
      setTimeout(() => {
        cookieConsent.classList.add("active")
      }, 2000)
  
      // Accept cookies
      cookieAccept.addEventListener("click", () => {
        localStorage.setItem("cookieConsent", "true")
        cookieConsent.classList.remove("active")
  
        // Remove after transition
        setTimeout(() => {
          cookieConsent.remove()
        }, 500)
  
        showToast("Cookies accepted!", "success")
      })
  
      // Decline cookies
      cookieDecline.addEventListener("click", () => {
        localStorage.setItem("cookieConsent", "false")
        cookieConsent.classList.remove("active")
  
        // Remove after transition
        setTimeout(() => {
          cookieConsent.remove()
        }, 500)
  
        showToast("Cookies declined. Some features may be limited.", "info")
      })
    }
  
    // Newsletter Popup
    function initNewsletterPopup() {
      // Check if user has already seen the popup
      if (localStorage.getItem("newsletterPopup")) {
        return
      }
  
      // Create newsletter popup HTML
      const newsletterPopupHTML = `
              <div class="newsletter-popup">
                  <div class="newsletter-content">
                      <button class="newsletter-close">&times;</button>
                      <div class="newsletter-image">
                          <h3>Join Our Newsletter</h3>
                      </div>
                      <div class="newsletter-body">
                          <p>Subscribe to get special offers, free giveaways, and once-in-a-lifetime deals.</p>
                          <form class="newsletter-form">
                              <input type="email" placeholder="Your email address" required>
                              <button type="submit">Subscribe</button>
                          </form>
                      </div>
                      <div class="newsletter-footer">
                          <p>By subscribing, you agree to our <a href="#">Privacy Policy</a>.</p>
                      </div>
                  </div>
              </div>
          `
  
      // Append to body
      document.body.insertAdjacentHTML("beforeend", newsletterPopupHTML)
  
      // Get elements
      const newsletterPopup = document.querySelector(".newsletter-popup")
      const newsletterClose = document.querySelector(".newsletter-close")
      const newsletterForm = document.querySelector(".newsletter-form")
  
      // Show newsletter popup after a delay
      setTimeout(() => {
        newsletterPopup.classList.add("active")
      }, 5000)
  
      // Close newsletter popup
      newsletterClose.addEventListener("click", () => {
        newsletterPopup.classList.remove("active")
  
        // Remove after transition
        setTimeout(() => {
          newsletterPopup.remove()
        }, 500)
  
        // Set flag in localStorage
        localStorage.setItem("newsletterPopup", "true")
      })
  
      // Click outside to close
      newsletterPopup.addEventListener("click", (e) => {
        if (e.target === newsletterPopup) {
          newsletterClose.click()
        }
      })
  
      // Form submission
      newsletterForm.addEventListener("submit", function (e) {
        e.preventDefault()
  
        const email = this.querySelector("input").value
  
        // In a real app, you would send this to your server
        console.log("Newsletter subscription:", email)
  
        // Close popup
        newsletterPopup.classList.remove("active")
  
        // Remove after transition
        setTimeout(() => {
          newsletterPopup.remove()
        }, 500)
  
        // Set flag in localStorage
        localStorage.setItem("newsletterPopup", "true")
  
        // Show success message
        showToast("Thank you for subscribing to our newsletter!", "success")
      })
    }
  
    // Product Image Zoom
    function initProductZoom() {
      const productImages = document.querySelectorAll(".product-image img")
  
      productImages.forEach((img) => {
        // Add zoom effect on hover
        img.addEventListener("mousemove", function (e) {
          const x = e.clientX - this.getBoundingClientRect().left
          const y = e.clientY - this.getBoundingClientRect().top
  
          const xPercent = (x / this.offsetWidth) * 100
          const yPercent = (y / this.offsetHeight) * 100
  
          this.style.transformOrigin = `${xPercent}% ${yPercent}%`
          this.style.transform = "scale(1.5)"
        })
  
        // Reset on mouse leave
        img.addEventListener("mouseleave", function () {
          this.style.transformOrigin = "center center"
          this.style.transform = "scale(1)"
        })
      })
    }
  
    // Animated Counters
    function initCounters() {
      // Create counters HTML
      const countersHTML = `
              <section class="counters-section">
                  <div class="container">
                      <div class="row text-center">
                          <div class="col-md-3 col-6">
                              <div class="counter reveal-up" data-count="1500">
                                  <i class="fas fa-users"></i>
                                  <h2 class="counter-value">0</h2>
                                  <p>Happy Customers</p>
                              </div>
                          </div>
                          <div class="col-md-3 col-6">
                              <div class="counter reveal-up" data-count="850" data-delay="200">
                                  <i class="fas fa-shopping-bag"></i>
                                  <h2 class="counter-value">0</h2>
                                  <p>Products</p>
                              </div>
                          </div>
                          <div class="col-md-3 col-6">
                              <div class="counter reveal-up" data-count="25" data-delay="400">
                                  <i class="fas fa-medal"></i>
                                  <h2 class="counter-value">0</h2>
                                  <p>Awards</p>
                              </div>
                          </div>
                          <div class="col-md-3 col-6">
                              <div class="counter reveal-up" data-count="15" data-delay="600">
                                  <i class="fas fa-store"></i>
                                  <h2 class="counter-value">0</h2>
                                  <p>Stores</p>
                              </div>
                          </div>
                      </div>
                  </div>
              </section>
          `
  
      // Find a suitable place to insert counters (e.g., before the footer)
      const footer = document.querySelector("footer")
      if (footer) {
        footer.insertAdjacentHTML("beforebegin", countersHTML)
      }
  
      // Animate counters when they come into view
      const counters = document.querySelectorAll(".counter")
  
      function animateCounters() {
        counters.forEach((counter) => {
          const counterValue = counter.querySelector(".counter-value")
          const target = Number.parseInt(counter.getAttribute("data-count"))
          const delay = Number.parseInt(counter.getAttribute("data-delay") || 0)
  
          const rect = counter.getBoundingClientRect()
          const isInView = rect.top <= window.innerHeight && rect.bottom >= 0
  
          if (isInView && !counter.classList.contains("animated")) {
            counter.classList.add("animated")
  
            setTimeout(() => {
              let count = 0
              const duration = 2000 // 2 seconds
              const interval = Math.floor(duration / target)
  
              const timer = setInterval(() => {
                count++
                counterValue.textContent = count
  
                if (count >= target) {
                  clearInterval(timer)
                }
              }, interval)
            }, delay)
          }
        })
      }
  
      // Initial check
      animateCounters()
  
      // Check on scroll
      window.addEventListener("scroll", animateCounters)
    }
  
    // Hover Animations
    function initHoverAnimations() {
      // Add hover classes to elements
      const productCards = document.querySelectorAll(".product-card")
      const categoryCards = document.querySelectorAll(".category-card")
      const buttons = document.querySelectorAll(".btn-primary")
  
      productCards.forEach((card) => {
        card.classList.add("hover-lift")
      })
  
      categoryCards.forEach((card) => {
        card.classList.add("hover-zoom")
      })
  
      buttons.forEach((button) => {
        button.classList.add("btn-animated")
      })
    }
  
    // Toast Notification System
    function initToastSystem() {
      // Create toast container if it doesn't exist
      if (!document.querySelector(".toast-container")) {
        const toastContainer = document.createElement("div")
        toastContainer.className = "toast-container"
        document.body.appendChild(toastContainer)
      }
    }
  
    // Show toast notification
    window.showToast = (message, type = "info", duration = 3000) => {
      // Get toast container
      let toastContainer = document.querySelector(".toast-container")
  
      if (!toastContainer) {
        toastContainer = document.createElement("div")
        toastContainer.className = "toast-container"
        document.body.appendChild(toastContainer)
      }
  
      // Create toast element
      const toast = document.createElement("div")
      toast.className = `toast toast-${type} animate-fade-in`
  
      // Set icon based on type
      let icon = "info-circle"
  
      switch (type) {
        case "success":
          icon = "check-circle"
          break
        case "error":
          icon = "times-circle"
          break
        case "warning":
          icon = "exclamation-triangle"
          break
      }
  
      // Set toast content
      toast.innerHTML = `
              <div class="toast-icon">
                  <i class="fas fa-${icon}"></i>
              </div>
              <div class="toast-content">
                  <div class="toast-message">${message}</div>
              </div>
              <button class="toast-close">&times;</button>
          `
  
      // Append toast to container
      toastContainer.appendChild(toast)
  
      // Close toast on button click
      toast.querySelector(".toast-close").addEventListener("click", () => {
        toast.classList.add("animate-fade-out")
  
        setTimeout(() => {
          toast.remove()
        }, 300)
      })
  
      // Auto close after duration
      setTimeout(() => {
        if (toast.parentNode) {
          toast.classList.add("animate-fade-out")
  
          setTimeout(() => {
            toast.remove()
          }, 300)
        }
      }, duration)
    }
  
    // Initialize Bootstrap Modals
    const bootstrap = window.bootstrap
  })
  
  