// contentScript.js

function extractProductData() {
  // Helpers
  const getText = (sel) => document.querySelector(sel)?.textContent?.trim() || '';
  const getAttr = (sel, attr) => document.querySelector(sel)?.getAttribute(attr) || '';

  // Get Price
  let price = 0;
  const priceText = getText('.a-price .a-offscreen') || getText('.a-price-whole');
  if (priceText) {
    price = parseFloat(priceText.replace(/[^0-9.]/g, ''));
  }

  // Get Category
  // Amazon breadcrumbs usually look like: Category > Sub > Item
  const breadcrumbs = Array.from(document.querySelectorAll('#wayfinding-breadcrumbs_feature_div li a'))
    .map(el => el.textContent.trim());
  
  // Get Image (High res if possible)
  let image = getAttr('#landingImage', 'src') || 
              getAttr('#imgBlkFront', 'src') || 
              getAttr('.a-dynamic-image', 'src');

  // If dynamic image, sometimes it's a JSON string in 'data-a-dynamic-image'
  const dynImg = getAttr('#landingImage', 'data-a-dynamic-image');
  if(dynImg) {
      try {
          const urls = Object.keys(JSON.parse(dynImg));
          if(urls.length > 0) image = urls[0]; // Get the first key (url)
      } catch(e) {}
  }

  const productData = {
    asin: getAttr('input[name="ASIN"]', 'value') || 'UNKNOWN',
    name: getText('#productTitle'),
    price: price,
    image: image,
    // Join categories to help our calculator find keywords like "Food" or "Electronics"
    category: breadcrumbs.join(' ') || getText('.a-color-tertiary') || 'General' 
  };

  if (productData.name) {
    chrome.storage.local.set({ currentProduct: productData });
  }
}

// Run on load and URL change
extractProductData();
let lastUrl = location.href;
new MutationObserver(() => {
  if (location.href !== lastUrl) {
    lastUrl = location.href;
    setTimeout(extractProductData, 1500); // Wait for SPA render
  }
}).observe(document, {subtree: true, childList: true});