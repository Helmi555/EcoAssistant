import React from 'react';

interface FeedbackModalProps {
  type: 'cart' | 'buy';
  rating: 'low' | 'medium' | 'high';
  onClose: () => void;
}

export const FeedbackModal: React.FC<FeedbackModalProps> = ({ type, rating, onClose }) => {
  const isGood = rating === 'low'; // Low carbon is good
  const isBad = rating === 'high';

  const getMessage = () => {
    if (type === 'cart') {
      if (isBad) return { title: "Wait!", text: "This item has a high carbon footprint. Consider a greener alternative?", color: "text-red-600", bg: "bg-red-50" };
      if (isGood) return { title: "Great Choice!", text: "This is an eco-friendly product. Adding to cart...", color: "text-green-600", bg: "bg-green-50" };
      return { title: "Notice", text: "This product has a moderate impact.", color: "text-yellow-600", bg: "bg-yellow-50" };
    } else {
      // Buy Now
      if (isBad) return { title: "Think Twice", text: "Buying this contributes significantly to emissions. Are you sure?", color: "text-red-700", bg: "bg-red-100" };
      if (isGood) return { title: "Thank You!", text: "Thanks for supporting sustainable products!", color: "text-green-700", bg: "bg-green-100" };
      return { title: "Purchase Recorded", text: "Your purchase has been logged.", color: "text-blue-600", bg: "bg-blue-50" };
    }
  };

  const content = getMessage();

  return (
    <div className="absolute inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-6 animate-in fade-in duration-200">
      <div className={`w-full bg-white rounded-2xl shadow-2xl p-6 text-center transform scale-100 ${content.bg}`}>
        <h3 className={`text-2xl font-bold mb-2 ${content.color}`}>{content.title}</h3>
        <p className="text-gray-700 mb-6 font-medium">{content.text}</p>
        <button 
          onClick={onClose}
          className="w-full py-3 bg-gray-900 text-white rounded-xl font-bold hover:bg-gray-800 transition-colors shadow-lg"
        >
          {type === 'cart' ? 'Continue Shopping' : 'Close'}
        </button>
      </div>
    </div>
  );
};