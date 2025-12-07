// contentScript.js
console.log('EcoAssistant: Content script loaded on Amazon');

// Listen for page changes (SPA navigation)
let lastUrl = location.href;
new MutationObserver(() => {
  const url = location.href;
  if (url !== lastUrl) {
    lastUrl = url;
    onPageChange();
  }
}).observe(document, { subtree: true, childList: true });

// Check if current page is a product page
function onPageChange() {
  if (isProductPage()) {
    extractProductData();
  }
}

// Check if we're on a product page
function isProductPage() {
  return window.location.href.includes('/dp/') || 
         window.location.href.includes('/gp/product/');
}

// Extract product data from current page
function extractProductData() {
  setTimeout(() => {
    const productData = getProductDetails();
    if (productData.asin) {
      // Store for popup to access
      chrome.storage.local.set({ currentProduct: productData });
      console.log('EcoAssistant: Product data extracted', productData);
    }
  }, 1000); // Wait for page to fully load
}
const getCategory = () => {
  // Get the full breadcrumb path from Amazon
  const breadcrumbs = Array.from(document.querySelectorAll('.a-breadcrumb a'));
  
  if (breadcrumbs.length > 0) {
    // Get all categories in the path
    const fullPath = breadcrumbs.map(a => a.textContent?.trim()).filter(Boolean);
    console.log('Amazon category path:', fullPath);
    
    // Return the full path for better classification
    return {
      main: fullPath[0] || 'General',
      sub: fullPath[1] || '',
      fullPath: fullPath,
      last: fullPath[fullPath.length - 1] || 'General'
    };
  }
  
  return {
    main: 'General',
    sub: '',
    fullPath: [],
    last: 'General'
  };
};
// Helper function to extract product details
function getProductDetails() {
  const getText = (selector) => 
    document.querySelector(selector)?.textContent?.trim() || '';
  
  const getPrice = () => {
    const priceWhole = document.querySelector('.a-price-whole')?.textContent;
    const priceFraction = document.querySelector('.a-price-fraction')?.textContent;
    if (priceWhole) {
      return parseFloat(priceWhole.replace(/[^0-9.]/g, '') + 
             (priceFraction ? '.' + priceFraction : ''));
    }
    return 0;
  };

  const getASIN = () => {
    // Extract ASIN from URL or page data
    const urlMatch = window.location.href.match(/\/dp\/([A-Z0-9]{10})/);
    if (urlMatch) return urlMatch[1];
    
    const dataAsin = document.querySelector('[data-asin]')?.getAttribute('data-asin');
    return dataAsin || '';
  };

  return {
    asin: getASIN(),
    name: getText('#productTitle'),
    price: getPrice(),
    image: document.querySelector('#landingImage')?.src || 
           document.querySelector('.a-dynamic-image')?.src ||
           '',
    category: Array.from(document.querySelectorAll('.a-breadcrumb a'))
      .map(a => a.textContent?.trim())
      .filter(Boolean)
      .pop() || 'General',
    color: document.querySelector('.selection li[aria-selected="true"]')?.textContent?.trim() || 
           document.querySelector('#color_name_0')?.textContent?.trim() ||
           'Default',
    url: window.location.href
  };
}

// Initial check
if (isProductPage()) {
  extractProductData();
}