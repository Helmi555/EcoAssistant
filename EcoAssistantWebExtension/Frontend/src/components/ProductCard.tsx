import React from 'react';

interface ProductCardProps {
  name: string;
  image: string;
  category: string;
  price: number;
}

export const ProductCard: React.FC<ProductCardProps> = ({ name, image, category, price }) => {
  return (
    <div className="flex flex-col items-center justify-center p-4 bg-white/50 rounded-xl backdrop-blur-sm shadow-sm mb-6">
      <div className="w-32 h-32 mb-3 p-2 bg-white rounded-lg shadow-inner flex items-center justify-center overflow-hidden">
        <img src={image} alt={name} className="max-w-full max-h-full object-contain" />
      </div>
      <h2 className="text-gray-800 font-bold text-center text-sm line-clamp-2 leading-tight mb-1">
        {name}
      </h2>
      <div className="flex justify-between w-full px-4 mt-2 text-xs text-gray-600 font-medium">
        <span className="bg-gray-200 px-2 py-1 rounded text-gray-700 truncate max-w-[120px]">{category}</span>
        <span className="text-gray-900 font-bold text-sm">${price}</span>
      </div>
    </div>
  );
};