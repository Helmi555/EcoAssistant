export interface Product {
  asin: string;
  name: string;
  price: number;
  image: string;
  category: string;
  color: string;
  url: string;
}

export interface CarbonFootprint {
  co2_kg: number;
  water_liters: number;
  energy_kwh: number;
  score: number; 
  factors: {
    material: number;
    transportation: number;
    manufacturing: number;
    packaging: number;
  };
}

export interface PurchaseRecord {
  id?: string;
  product_asin: string;
  product_name: string;
  quantity: number;
  color: string;
  carbon_footprint: number;
  purchase_date: Date;
  user_id: string;
}