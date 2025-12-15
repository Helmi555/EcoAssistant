import React, { useState, useEffect } from 'react';
import { ProductCard } from './components/ProductCard';
import { StatsPanel } from './components/StatsPanel';
import { FeedbackModal } from './components/FeedbackModal';
import { calculateCarbon, CarbonResult } from './utils/carbonCalculator';

// Types
interface Product {
  asin: string;
  name: string;
  price: number;
  image: string;
  category: string;
}

// 1. Define Props Interface
interface PopUpProps {
  onLogout: () => void;
}

// 2. Accept the prop
const PopUp: React.FC<PopUpProps> = ({ onLogout }) => {
  const [product, setProduct] = useState<Product | null>(null);
  const [quantity, setQuantity] = useState<number>(1);
  const [carbonData, setCarbonData] = useState<CarbonResult | null>(null);
  const [modalState, setModalState] = useState<{ show: boolean, type: 'cart' | 'buy' }>({ show: false, type: 'cart' });
  const [loading, setLoading] = useState(true);

  // Load product from Chrome Storage
  useEffect(() => {
    const fetchProduct = () => {
      // Check if chrome API exists (extension environment)
      if (typeof chrome !== 'undefined' && chrome.storage) {
        chrome.storage.local.get(['currentProduct'], (result) => {
          if (result.currentProduct) {
            setProduct(result.currentProduct);
          }
          setLoading(false);
        });
      } else {
        // Mock data for development
        setProduct({
            asin: '123',
            name: 'Organic Extra Virgin Olive Oil (1L)',
            price: 24.99,
            image: 'https://m.media-amazon.com/images/I/81+2+12+34.jpg', // Placeholder
            category: 'Grocery & Gourmet Food'
        });
        setLoading(false);
      }
    };

    fetchProduct();
  }, []);

  // Recalculate when Product or Quantity changes
  useEffect(() => {
    if (product) {
      const result = calculateCarbon(product.category, product.name, product.price, quantity);
      setCarbonData(result);
    }
  }, [product, quantity]);

  // Determine dynamic background class
  const getThemeGradient = () => {
    if (!carbonData) return 'bg-gray-100';
    switch (carbonData.rating) {
      case 'low': return 'bg-gradient-to-br from-emerald-100 to-green-300'; // Green
      case 'medium': return 'bg-gradient-to-br from-amber-50 to-orange-200'; // Yellow/Orange
      case 'high': return 'bg-gradient-to-br from-rose-100 to-red-300'; // Red
      default: return 'bg-gray-100';
    }
  };

  const handleAction = (type: 'cart' | 'buy') => {
    setModalState({ show: true, type });
  };

  if (loading) return <div className="w-[350px] h-[500px] flex items-center justify-center">Loading...</div>;

  if (!product || !carbonData) return (
    <div className="w-[350px] h-[500px] flex flex-col items-center justify-center text-center p-6 bg-gray-50">
      <p className="text-gray-500 mb-4">No Amazon product detected.</p>
      <button 
        onClick={onLogout}
        className="text-sm text-red-500 hover:text-red-700 underline"
      >
        Logout
      </button>
    </div>
  );

  return (
    <div className={`w-[350px] min-h-[550px] relative overflow-hidden transition-colors duration-500 ${getThemeGradient()}`}>
      
      {/* Feedback Modal Overlay */}
      {modalState.show && (
        <FeedbackModal 
          type={modalState.type} 
          rating={carbonData.rating} 
          onClose={() => setModalState({ ...modalState, show: false })} 
        />
      )}

      {/* Main Content */}
      <div className="p-6 h-full flex flex-col">
        {/* Header */}
        <div className="flex justify-between items-center mb-4">
            <h1 className="text-xl font-black text-gray-800 tracking-tight">EcoAssistant</h1>
            
            {/* 3. Logout Button */}
            <button 
              onClick={onLogout}
              className="flex items-center gap-1 bg-white/40 hover:bg-white/60 px-3 py-1.5 rounded-lg text-xs font-bold text-gray-700 transition-colors border border-black/5"
            >
              <svg xmlns="http://www.w3.org/2000/svg" className="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
              Logout
            </button>
        </div>

        <ProductCard 
          name={product.name}
          image={product.image}
          category={product.category}
          price={product.price}
        />

        <StatsPanel data={carbonData} />

        {/* Dynamic Quantity Input */}
        <div className="bg-white/40 rounded-xl p-3 mb-6 flex items-center justify-between">
            <label className="text-sm font-bold text-gray-700">Quantity</label>
            <div className="flex items-center gap-3">
                <button 
                  onClick={() => setQuantity(Math.max(1, quantity - 1))}
                  className="w-8 h-8 rounded-full bg-white shadow flex items-center justify-center font-bold text-gray-600 hover:bg-gray-50"
                >-</button>
                <span className="font-bold text-lg w-6 text-center">{quantity}</span>
                <button 
                  onClick={() => setQuantity(quantity + 1)}
                  className="w-8 h-8 rounded-full bg-white shadow flex items-center justify-center font-bold text-gray-600 hover:bg-gray-50"
                >+</button>
            </div>
        </div>

        {/* Action Buttons */}
        <div className="mt-auto grid grid-cols-2 gap-3">
            <button 
                onClick={() => handleAction('cart')}
                className="py-3 px-4 bg-white/80 hover:bg-white text-gray-800 font-bold rounded-xl shadow-sm transition-all text-sm border border-gray-200"
            >
                Add to Cart
            </button>
            <button 
                onClick={() => handleAction('buy')}
                className="py-3 px-4 bg-gray-900 hover:bg-black text-white font-bold rounded-xl shadow-lg transition-all text-sm"
            >
                Buy Now
            </button>
        </div>
      </div>
    </div>
  );
};

export default PopUp;