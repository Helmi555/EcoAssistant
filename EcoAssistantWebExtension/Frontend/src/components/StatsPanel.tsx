import React from 'react';
import { CarbonResult } from '../utils/carbonCalculator';

interface StatsPanelProps {
  data: CarbonResult;
}

export const StatsPanel: React.FC<StatsPanelProps> = ({ data }) => {
  return (
    <div className="grid grid-cols-2 gap-3 mb-6">
      <div className="bg-white/60 p-3 rounded-xl border border-white/50 text-center">
        <p className="text-xs uppercase tracking-wider text-gray-500 font-semibold">Footprint</p>
        <p className="text-2xl font-black text-gray-800">
          {data.co2_kg}<span className="text-sm font-normal text-gray-600">kg</span>
        </p>
        <p className="text-[10px] text-gray-500">CO₂ Emissions</p>
      </div>
      
      <div className="bg-white/60 p-3 rounded-xl border border-white/50 text-center">
        <p className="text-xs uppercase tracking-wider text-gray-500 font-semibold">Eco Score</p>
        <div className="flex items-center justify-center gap-1">
          <p className={`text-2xl font-black ${
            data.score >= 7 ? 'text-green-600' : 
            data.score >= 4 ? 'text-yellow-600' : 'text-red-600'
          }`}>
            {data.score}
          </p>
          <span className="text-gray-400 text-sm">/10</span>
        </div>
        <p className="text-[10px] text-gray-500">Environmental Impact</p>
      </div>
    </div>
  );
};