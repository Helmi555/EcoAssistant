// categoryMapping.ts

// Amazon category to our carbon type mapping
export const amazonCategoryMapping: { [key: string]: string } = {
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
  'Exercise & Fitness':'sports'
}