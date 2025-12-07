import React, { useState, useEffect } from 'react';
import { Product, CarbonFootprint, PurchaseRecord } from './product';

interface PopUpProps {
  onLogout: () => void;
  authToken: string | null;
}

// Check if we're in a Chrome extension context
const isChromeExtension = typeof chrome !== 'undefined' && chrome.storage;

// Category mapping for Amazon categories
const amazonCategoryMapping: { [key: string]: string } = {
  // Food & Grocery
  'Grocery & Gourmet Food': 'food',
  'Food & Beverage': 'food',
  'Pantry Staples': 'food',
  'Snack Foods': 'food',
  'Beverages': 'food',
  'Coffee, Tea & Cocoa': 'food',
  'Cooking & Baking': 'food',
  'Olive Oil': 'food',
  'Pasta & Noodles': 'food',
  'Rice & Grains': 'food',
  'Fruits & Vegetables': 'food',
  'Meat & Seafood': 'food',
  'Dairy & Eggs': 'food',
  
  // Electronics
  'Electronics': 'electronics',
  'Computers & Accessories': 'electronics',
  'Computer Accessories': 'electronics',
  'Mousepads': 'electronics',
  'Keyboards': 'electronics',
  'Mice': 'electronics',
  'Cell Phones & Accessories': 'electronics',
  'TV & Video': 'electronics',
  'Camera & Photo': 'electronics',
  'Audio & Headphones': 'electronics',
  'Video Games': 'electronics',
  
  // Clothing
  'Clothing, Shoes & Jewelry': 'clothing',
  'Men': 'clothing',
  'Women': 'clothing',
  'Kids': 'clothing',
  'Shoes': 'clothing',
  'Jewelry': 'clothing',
  'Watches': 'clothing',
  
  // Books
  'Books': 'books',
  'Kindle Store': 'books',
  'Textbooks': 'books',
  'Audible Books & Originals': 'books',
  
  // Home & Kitchen
  'Home & Kitchen': 'home',
  'Kitchen & Dining': 'home',
  'Bedding': 'home',
  'Bath': 'home',
  'Furniture': 'home',
  'Home Improvement': 'home',
  'Tools & Home Improvement': 'home',
  
  // Health & Beauty
  'Beauty & Personal Care': 'health',
  'Health & Household': 'health',
  'Personal Care': 'health',
  
  // Sports & Outdoors
  'Sports & Outdoors': 'sports',
  'Exercise & Fitness': 'sports',
  'Outdoor Recreation': 'sports',
  
  // Toys & Games
  'Toys & Games': 'toys',
  'Baby': 'toys',
  
  // Automotive
  'Automotive': 'automotive',
  'Car & Vehicle Electronics': 'automotive',
  
  // Office Products
  'Office Products': 'office',
  'Office & School Supplies': 'office',
  
  // Pet Supplies
  'Pet Supplies': 'pets',
  
  // Industrial & Scientific
  'Industrial & Scientific': 'industrial',
  
  // Default
  'General': 'general'
};

// Carbon multipliers per dollar for each product type
const carbonMultipliers: { [key: string]: number } = {
  'food': 0.1,          // $1 = 0.1 kg CO₂ (food is low carbon)
  'electronics': 0.5,   // $1 = 0.5 kg CO₂ (electronics high)
  'clothing': 0.3,      // $1 = 0.3 kg CO₂ (clothing moderate)
  'books': 0.2,         // $1 = 0.2 kg CO₂ (books low)
  'home': 0.4,          // $1 = 0.4 kg CO₂ (home goods moderate)
  'health': 0.35,       // $1 = 0.35 kg CO₂
  'sports': 0.4,        // $1 = 0.4 kg CO₂
  'toys': 0.35,         // $1 = 0.35 kg CO₂
  'automotive': 0.6,    // $1 = 0.6 kg CO₂ (cars high)
  'office': 0.3,        // $1 = 0.3 kg CO₂
  'pets': 0.25,         // $1 = 0.25 kg CO₂
  'industrial': 0.7,    // $1 = 0.7 kg CO₂ (industrial very high)
  'general': 0.35,      // $1 = 0.35 kg CO₂ (default)
};

// Food-specific multipliers
const foodMultipliers: { [key: string]: number } = {
  'olive_oil': 0.8,
  'cooking_oil': 0.9,
  'pasta': 0.6,
  'grains': 0.5,
  'coffee': 1.2,
  'tea': 0.8,
  'chocolate': 1.5,
  'fruits': 0.3,
  'vegetables': 0.3,
  'meat': 2.5,      // Meat has very high carbon!
  'seafood': 1.8,
  'dairy': 1.5,
  'bakery': 1.2,
  'snacks': 1.3,
  'processed': 1.4,
  'general_food': 1.0,
};

// Helper function to map Amazon category to product type
const mapAmazonCategory = (amazonCategory: string | string[]): string => {
  if (Array.isArray(amazonCategory)) {
    // Check each category in the path
    for (const category of amazonCategory) {
      const mapped = amazonCategoryMapping[category];
      if (mapped) return mapped;
    }
    return 'general';
  } else {
    return amazonCategoryMapping[amazonCategory] || 'general';
  }
};

// Detect food type from product name and category
const detectFoodType = (productName: string, categories: string[]): string => {
  const name = productName.toLowerCase();
  
  // First check category
  for (const category of categories) {
    const lowerCat = category.toLowerCase();
    if (lowerCat.includes('olive') && lowerCat.includes('oil')) return 'olive_oil';
    if (lowerCat.includes('meat') || lowerCat.includes('beef') || lowerCat.includes('chicken')) return 'meat';
    if (lowerCat.includes('dairy') || lowerCat.includes('milk') || lowerCat.includes('cheese')) return 'dairy';
    if (lowerCat.includes('fruit')) return 'fruits';
    if (lowerCat.includes('vegetable')) return 'vegetables';
    if (lowerCat.includes('seafood') || lowerCat.includes('fish')) return 'seafood';
    if (lowerCat.includes('coffee')) return 'coffee';
    if (lowerCat.includes('tea')) return 'tea';
    if (lowerCat.includes('chocolate')) return 'chocolate';
    if (lowerCat.includes('pasta')) return 'pasta';
    if (lowerCat.includes('rice')) return 'grains';
  }
  
  // Then check product name
  if (name.includes('olive oil')) return 'olive_oil';
  if (name.includes('beef') || name.includes('steak') || name.includes('chicken') || name.includes('pork')) return 'meat';
  if (name.includes('cheese') || name.includes('milk') || name.includes('yogurt')) return 'dairy';
  if (name.includes('apple') || name.includes('banana') || name.includes('orange')) return 'fruits';
  if (name.includes('broccoli') || name.includes('carrot') || name.includes('lettuce')) return 'vegetables';
  if (name.includes('salmon') || name.includes('tuna') || name.includes('shrimp')) return 'seafood';
  if (name.includes('coffee')) return 'coffee';
  if (name.includes('tea')) return 'tea';
  if (name.includes('chocolate')) return 'chocolate';
  if (name.includes('pasta')) return 'pasta';
  if (name.includes('rice')) return 'grains';
  
  return 'general_food';
};

// Get category multiplier based on product type
const getCategoryMultiplier = (productType: string): number => {
  const multipliers: { [key: string]: number } = {
    'electronics': 1.5,
    'clothing': 0.8,
    'books': 0.3,
    'home': 1.2,
    'general': 1.0,
    'food': 1.0,  // Food uses foodMultipliers instead
    'health': 1.1,
    'sports': 1.3,
    'toys': 1.1,
    'automotive': 1.8,
    'office': 0.9,
    'pets': 0.7,
    'industrial': 2.0,
  };
  return multipliers[productType] || 1.0;
};

function PopUp({ onLogout, authToken }: PopUpProps): JSX.Element {
  const [currentProduct, setCurrentProduct] = useState<Product | null>(null);
  const [carbonData, setCarbonData] = useState<CarbonFootprint | null>(null);
  const [quantity, setQuantity] = useState<number>(1);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isOnAmazon, setIsOnAmazon] = useState<boolean>(false);

  useEffect(() => {
    if (isChromeExtension) {
      loadCurrentProduct();
      checkCurrentTab();
      
      // Listen for storage changes when product switches
      const handleStorageChange = (changes: { [key: string]: chrome.storage.StorageChange }) => {
        console.log('Storage changed:', changes);
        if (changes.currentProduct) {
          console.log('New product detected:', changes.currentProduct.newValue);
          setCurrentProduct(changes.currentProduct.newValue);
          if (changes.currentProduct.newValue) {
            calculateCarbonFootprint(changes.currentProduct.newValue);
          }
        }
      };
      
      // Setup listener
      chrome.storage.onChanged.addListener(handleStorageChange);
      
      // Cleanup
      return () => {
        chrome.storage.onChanged.removeListener(handleStorageChange);
      };
    } else {
      setCurrentProduct(getMockProduct());
    }
  }, []);

  const loadCurrentProduct = async () => {
    if (!isChromeExtension) return;
    
    chrome.storage.local.get(['currentProduct'], (result) => {
      if (result.currentProduct) {
        setCurrentProduct(result.currentProduct);
        calculateCarbonFootprint(result.currentProduct);
      } else {
        // No product found in storage
        setCurrentProduct(null);
      }
    });
  };

  const checkCurrentTab = async () => {
    if (!isChromeExtension) return;
    
    try {
      const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
      setIsOnAmazon(tab.url?.includes('amazon.com') || false);
    } catch (error) {
      console.error('Error checking current tab:', error);
    }
  };

  const calculateCarbonFootprint = async (product: Product) => {
    setIsLoading(true);
    try {
      // In development, use mock data
      if (!authToken || !isChromeExtension) {
        setTimeout(() => {
          setCarbonData(calculateFallbackCarbon(product));
          setIsLoading(false);
        }, 1000);
        return;
      }

      // Production API call
      const response = await fetch('http://localhost:5000/api/carbon/calculate', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${authToken}`
        },
        body: JSON.stringify(product)
      });
      
      if (response.ok) {
        const carbonData: CarbonFootprint = await response.json();
        setCarbonData(carbonData);
      } else {
        throw new Error('API request failed');
      }
    } catch (error) {
      console.error('Error calculating carbon footprint:', error);
      // Fallback calculation
      setCarbonData(calculateFallbackCarbon(product));
    } finally {
      setIsLoading(false);
    }
  };

  const calculateFallbackCarbon = (product: Product): CarbonFootprint => {
    // Get category array from product
    let categoryArray: string[] = [];
    if (Array.isArray(product.category)) {
      categoryArray = product.category;
    } else if (typeof product.category === 'string') {
      categoryArray = [product.category];
    }
    
    // Map Amazon category to our product type
    const productType = mapAmazonCategory(categoryArray);
    
    // Get base carbon per dollar for this product type
    const baseCarbonPerDollar = carbonMultipliers[productType] || 0.35;
    let baseCO2 = product.price * baseCarbonPerDollar;
    
    // Apply special handling for food products
    if (productType === 'food') {
      const foodType = detectFoodType(product.name, categoryArray);
      const foodMultiplier = foodMultipliers[foodType] || 1.0;
      baseCO2 = baseCO2 * foodMultiplier;
    } else {
      // Apply category multiplier for non-food products
      const categoryMultiplier = getCategoryMultiplier(productType);
      baseCO2 = baseCO2 * categoryMultiplier;
    }
    
    // Calculate water and energy usage based on product type
    const { waterLiters, energyKwh } = calculateResourceUsage(productType, baseCO2);
    
    // Calculate eco score (1-10)
    const score = calculateEcoScore(baseCO2, productType);
    
    return {
      co2_kg: baseCO2,
      water_liters: waterLiters,
      energy_kwh: energyKwh,
      score: score,
      factors: {
        material: getMaterialFactor(productType),
        transportation: 0.3,
        manufacturing: 0.3,
        packaging: 0.2
      }
    };
  };

  const calculateResourceUsage = (productType: string, co2: number) => {
    // Different products use different amounts of water and energy
    const waterPerCO2 = {
      'food': 25,        // Agriculture uses lots of water
      'clothing': 35,    // Textile production is water-intensive
      'electronics': 3,  // Electronics use little water
      'general': 10,
      'home': 8,
      'books': 5,
      'health': 12,
      'sports': 15,
      'toys': 10,
      'automotive': 6,
      'office': 7,
      'pets': 20,
      'industrial': 4,
    };
    
    const energyPerCO2 = {
      'food': 0.8,       // Food processing uses some energy
      'clothing': 1.2,   // Clothing manufacturing moderate
      'electronics': 2.5, // Electronics very energy-intensive
      'general': 1.5,
      'home': 1.8,
      'books': 0.5,
      'health': 1.3,
      'sports': 1.6,
      'toys': 1.4,
      'automotive': 2.2,
      'office': 1.1,
      'pets': 0.9,
      'industrial': 3.0,
    };
    
    return {
      waterLiters: co2 * (waterPerCO2[productType] || 10),
      energyKwh: co2 * (energyPerCO2[productType] || 1.5)
    };
  };

  const calculateEcoScore = (co2_kg: number, productType: string): number => {
    // Different scoring for different product types
    const scoreRanges = {
      'food': { max: 9, divisor: 0.8 },        // Food can score higher
      'electronics': { max: 7, divisor: 15 },   // Electronics score lower
      'clothing': { max: 8, divisor: 10 },
      'books': { max: 10, divisor: 2 },        // Books can score very high
      'general': { max: 8, divisor: 8 }
    };
    
    const range = scoreRanges[productType] || scoreRanges.general;
    let score = range.max - (co2_kg / range.divisor);
    
    // Clamp between 1 and 10
    return Math.max(1, Math.min(10, Math.round(score * 10) / 10));
  };

  const getMaterialFactor = (productType: string): number => {
    // Material impact factor (0-1)
    const factors = {
      'food': 0.2,        // Mostly natural materials
      'electronics': 0.6, // Many rare materials
      'clothing': 0.4,   // Fabric materials
      'books': 0.1,      // Mostly paper
      'home': 0.5,
      'general': 0.3
    };
    return factors[productType] || 0.3;
  };

  const recordPurchase = async () => {
    if (!currentProduct || !carbonData) return;

    try {
      const purchaseRecord: PurchaseRecord = {
        product_asin: currentProduct.asin,
        product_name: currentProduct.name,
        quantity: quantity,
        color: currentProduct.color,
        carbon_footprint: carbonData.co2_kg * quantity,
        purchase_date: new Date(),
        user_id: authToken || 'demo-user'
      };

      // In development, just show success message
      if (!authToken || !isChromeExtension) {
        alert('Purchase recorded! (Demo mode)');
        return;
      }

      await fetch('http://localhost:5000/api/purchases', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${authToken}`
        },
        body: JSON.stringify(purchaseRecord)
      });

      alert('Purchase recorded! Thank you for being eco-conscious!');
    } catch (error) {
      console.error('Error recording purchase:', error);
      alert('Demo: Purchase would be recorded in production');
    }
  };

  const refreshProductData = async () => {
    if (!isChromeExtension) {
      setCurrentProduct(getMockProduct());
      return;
    }

    const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
    if (tab.id && tab.url?.includes('amazon.com')) {
      try {
        // Ask content script to re-extract data
        await chrome.tabs.sendMessage(tab.id, { action: 'extractProduct' });
        // Reload after a delay
        setTimeout(loadCurrentProduct, 1000);
      } catch (error) {
        console.error('Error refreshing product data:', error);
      }
    }
  };

  // Development mock data
  const getMockProduct = (): Product => ({
    asin: 'B0ABCD1234',
    name: 'Example Product - Wireless Headphones',
    price: 99.99,
    image: 'https://via.placeholder.com/150',
    category: 'Electronics',
    color: 'Black',
    url: 'https://www.amazon.com/dp/B0ABCD1234'
  });

  // Show different states based on context
  if (!isChromeExtension) {
    // Development mode - show mock data
    return (
      <div className="min-w-[380px] min-h-[500px] bg-gradient-to-br from-blue-50 to-green-50 p-6">
        <div className="bg-white rounded-2xl shadow-xl border border-gray-100 p-6">
          <div className="text-center mb-4">
            <div className="bg-yellow-100 border border-yellow-400 text-yellow-800 px-4 py-2 rounded-lg mb-4">
              <strong>Development Mode</strong> - Using mock data
            </div>
            <h1 className="text-2xl font-bold text-gray-800 mb-2">Eco Assistant</h1>
            <p className="text-gray-600">Carbon Footprint Analysis</p>
          </div>
          {renderProductContent()}
        </div>
      </div>
    );
  }

  if (!isOnAmazon) {
    return (
      <div className="min-w-[380px] min-h-[500px] bg-gradient-to-br from-blue-50 to-green-50 p-6 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-bold text-gray-800 mb-2">Not on Amazon</h2>
          <p className="text-gray-600">Navigate to Amazon.com to analyze products</p>
        </div>
      </div>
    );
  }

  if (!currentProduct) {
    return (
      <div className="min-w-[380px] min-h-[500px] bg-gradient-to-br from-blue-50 to-green-50 p-6 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-bold text-gray-800 mb-2">No Product Detected</h2>
          <p className="text-gray-600 mb-4">Navigate to an Amazon product page</p>
          <button
            onClick={refreshProductData}
            className="bg-green-500 hover:bg-green-600 text-white py-2 px-4 rounded-lg font-semibold transition duration-200"
          >
            Refresh
          </button>
        </div>
      </div>
    );
  }

  function renderProductContent() {
    // Format category for display
    let categoryDisplay = '';
    if (Array.isArray(currentProduct!.category)) {
      categoryDisplay = currentProduct!.category.join(' > ');
    } else {
      categoryDisplay = currentProduct!.category;
    }
    
    // Get product type for display
    let productType = 'general';
    if (Array.isArray(currentProduct!.category)) {
      productType = mapAmazonCategory(currentProduct!.category);
    } else if (typeof currentProduct!.category === 'string') {
      productType = mapAmazonCategory([currentProduct!.category]);
    }

    return (
      <>
        {/* Product Info */}
        <div className="mb-6 p-4 bg-gray-50 rounded-lg">
          <div className="flex items-center space-x-4">
            <img 
              src={currentProduct!.image} 
              alt={currentProduct!.name}
              className="w-16 h-16 object-cover rounded"
            />
            <div className="flex-1">
              <h3 className="font-semibold text-gray-800 text-sm line-clamp-2">
                {currentProduct!.name}
              </h3>
              <p className="text-green-600 font-bold">${currentProduct!.price.toFixed(2)}</p>
              <div className="text-xs text-gray-500 mt-1">
                <div>Category: {categoryDisplay}</div>
                {carbonData && (
                  <div className="mt-1">
                    Product Type: <span className="font-medium">{productType}</span>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>

        {/* Carbon Footprint Results */}
        {isLoading ? (
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-green-500 mx-auto"></div>
            <p className="mt-4 text-gray-600">Calculating carbon footprint...</p>
          </div>
        ) : carbonData && (
          <div className="space-y-4 mb-6">
            <div className="bg-red-50 border border-red-200 rounded-lg p-4">
              <h3 className="font-semibold text-red-800 mb-2">Carbon Footprint</h3>
              <p className="text-2xl font-bold text-red-600">{carbonData.co2_kg.toFixed(2)} kg CO₂</p>
              <p className="text-sm text-red-700 mt-1">
                Equivalent to driving {(carbonData.co2_kg * 0.4).toFixed(1)} miles
              </p>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                <p className="text-sm text-blue-700">Water Usage</p>
                <p className="font-bold text-blue-800">{carbonData.water_liters.toFixed(0)}L</p>
              </div>
              <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3">
                <p className="text-sm text-yellow-700">Energy</p>
                <p className="font-bold text-yellow-800">{carbonData.energy_kwh.toFixed(1)} kWh</p>
              </div>
            </div>

            {/* Eco Score */}
            <div className="bg-green-50 border border-green-200 rounded-lg p-4">
              <div className="flex justify-between items-center mb-2">
                <span className="font-semibold text-green-800">Eco Score</span>
                <span className="text-lg font-bold text-green-600">{carbonData.score.toFixed(1)}/10</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div 
                  className="bg-green-500 h-2 rounded-full transition-all duration-300" 
                  style={{ width: `${carbonData.score * 10}%` }}
                ></div>
              </div>
              <p className="text-xs text-green-700 mt-2">
                {carbonData.score >= 8 ? 'Excellent' : 
                 carbonData.score >= 6 ? 'Good' : 
                 carbonData.score >= 4 ? 'Average' : 'Poor'} environmental impact
              </p>
            </div>
          </div>
        )}

        {/* Purchase Recording */}
        <div className="space-y-4">
          <div className="flex items-center space-x-4">
            <label className="text-sm font-medium text-gray-700">Quantity:</label>
            <input
              type="number"
              min="1"
              value={quantity}
              onChange={(e) => setQuantity(Math.max(1, parseInt(e.target.value) || 1))}
              className="w-20 px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
            />
          </div>

          <button
            onClick={recordPurchase}
            className="w-full bg-green-500 hover:bg-green-600 text-white py-3 px-4 rounded-lg font-semibold transition duration-200"
          >
            {isChromeExtension ? 'Record Purchase' : 'Demo: Record Purchase'}
          </button>
        </div>

        {/* Logout Button */}
        <button
          onClick={onLogout}
          className="w-full mt-4 bg-gray-500 hover:bg-gray-600 text-white py-2 px-4 rounded-lg font-semibold transition duration-200"
        >
          Logout
        </button>
      </>
    );
  }

  return (
    <div className="min-w-[380px] min-h-[500px] bg-gradient-to-br from-blue-50 to-green-50 p-6">
      <div className="bg-white rounded-2xl shadow-xl border border-gray-100 p-6">
        {/* Header */}
        <div className="text-center mb-6">
          <h1 className="text-2xl font-bold text-gray-800 mb-2">
            Eco Assistant
          </h1>
          <p className="text-gray-600">Carbon Footprint Analysis</p>
        </div>
        {renderProductContent()}
      </div>
    </div>
  );
}

export default PopUp;