import { useState } from 'react';

function Signup() {
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [confirmPassword, setConfirmPassword] = useState<string>("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (password !== confirmPassword) {
      alert("Passwords don't match!");
      return;
    }
    
    // Handle signup logic
    console.log({ email, password });
  };

  return (
    <div className="warning">
      <h2>Create Account</h2>
      <p>Please sign up to use EcoAssistant.</p>
      
      <form onSubmit={handleSubmit}>
        <label htmlFor="signup-email">Email</label>
        <input 
          type="email" 
          id="signup-email"
          value={email}
          onChange={(e) => setEmail(e.target.value)} 
          required
        />
        
        <label htmlFor="signup-password">Password</label>
        <input 
          type="password" 
          id="signup-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        
        <label htmlFor="confirm-password">Confirm Password</label>
        <input 
          type="password" 
          id="confirm-password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required
        />
        
        <button type="submit">Sign Up</button>
      </form>
      
      <p>If you already have an account <a href="#login">Login</a></p>
    </div>
  );
}

export default Signup;