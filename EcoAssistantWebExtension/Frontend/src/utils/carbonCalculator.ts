// src/utils/carbonCalculator.ts

export interface CarbonResult {
  co2_kg: number;
  score: number; // 1-10 (10 is best)
  rating: 'low' | 'medium' | 'high';
  water_liters: number;
}

// Base emission estimates (kg CO2) for a typical unit in this category
const CATEGORY_BASE_EMISSIONS: { [key: string]: number } = {
  food: 3.0,
  electronics: 80.0, // High manufacturing cost
  clothing: 15.0,
  books: 2.0,
  home: 20.0,
  beauty: 5.0,
  automotive: 150.0,
  general: 10.0
};

// Modifiers based on specific keywords found in the title
const KEYWORD_MODIFIERS: { [key: string]: number } = {
  // Food High Impact
  'beef': 30.0,
  'steak': 30.0,
  'lamb': 25.0,
  'pork': 12.0,
  'cheese': 15.0,
  'coffee': 10.0,
  
  // Food Low Impact
  'vegetable': -2.0,
  'fruit': -2.0,
  'chicken': 3.0,
  'olive oil': 1.5, // Specific fix for your olive oil issue
  'pasta': -1.0,
  'bread': -1.5,

  // Electronics
  'laptop': 150.0,
  'desktop': 200.0,
  'monitor': 100.0,
  'smartphone': 60.0,
  'cable': -70.0, // Adjust down from electronics base
  
  // Clothing
  'cotton': -2.0,
  'polyester': 5.0,
  'leather': 40.0,
};

export const calculateCarbon = (
  category: string, 
  name: string, 
  price: number,
  quantity: number
): CarbonResult => {
  const normalizedName = name.toLowerCase();
  const normalizedCat = category.toLowerCase();

  // 1. Determine Broad Category
  let baseCategory = 'general';
  if (normalizedCat.includes('grocery') || normalizedCat.includes('food')) baseCategory = 'food';
  else if (normalizedCat.includes('electronics') || normalizedCat.includes('computer')) baseCategory = 'electronics';
  else if (normalizedCat.includes('clothing') || normalizedCat.includes('apparel')) baseCategory = 'clothing';
  else if (normalizedCat.includes('beauty') || normalizedCat.includes('health')) baseCategory = 'beauty';
  else if (normalizedCat.includes('automotive')) baseCategory = 'automotive';

  // 2. Start with Base Emission
  let co2PerUnit = CATEGORY_BASE_EMISSIONS[baseCategory] || 10.0;

  // 3. Apply Keyword Modifiers (This fixes the Olive Oil vs Beef issue)
  let keywordMatch = false;
  for (const [key, mod] of Object.entries(KEYWORD_MODIFIERS)) {
    if (normalizedName.includes(key)) {
      if (keywordMatch) {
         // If we already matched a keyword, take the average of modifiers
         co2PerUnit = (co2PerUnit + mod) / 2;
      } else {
         // If specific keyword found, override or adjust base heavily
         co2PerUnit = mod > 0 ? mod : Math.max(0.5, co2PerUnit + mod);
      }
      keywordMatch = true;
    }
  }

  // 4. Price Logic (Subtle adjustment only)
  // If no keyword matched, use price to guess size/complexity within category
  if (!keywordMatch) {
    // E.g. $2000 laptop likely heavier carbon than $200 laptop, but not linear
    co2PerUnit = co2PerUnit * (1 + (Math.log10(Math.max(1, price)) * 0.2));
  }

  const totalCo2 = co2PerUnit * quantity;

  // 5. Calculate Score (0-10)
  // Thresholds: <5kg = Great, <20kg = OK, >20kg = Bad
  let score = 10 - (totalCo2 / 5); 
  score = Math.max(1, Math.min(10, score)); // Clamp 1-10

  // 6. Determine Rating
  let rating: 'low' | 'medium' | 'high' = 'medium';
  if (score >= 7) rating = 'low'; // Low carbon = Good score
  else if (score <= 4) rating = 'high';

  return {
    co2_kg: parseFloat(totalCo2.toFixed(2)),
    score: parseFloat(score.toFixed(1)),
    rating,
    water_liters: parseFloat((totalCo2 * 50).toFixed(0)) // Rough est
  };
};