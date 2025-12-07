import { useState, useEffect } from 'react';
import Login from './Login';
import PopUp from './PopUp';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);
  const [authToken, setAuthToken] = useState<string | null>(null);

  // Check if user is already logged in when component mounts
  useEffect(() => {
    const token = localStorage.getItem('authToken');
    if (token) {
      setAuthToken(token);
      setIsLoggedIn(true);
    }
  }, []);

  const handleLoginSuccess = (): void => {
    setIsLoggedIn(true);
    const token = localStorage.getItem('authToken');
    setAuthToken(token);
  };

  const handleLogout = (): void => {
    localStorage.removeItem('authToken');
    setAuthToken(null);
    setIsLoggedIn(false);
  };

  // Render the appropriate component based on login state
  return isLoggedIn ? (
    <PopUp onLogout={handleLogout} authToken={authToken} />
  ) : (
    <Login onLoginSuccess={handleLoginSuccess} />
  );
}

export default App;