import React, { useState } from 'react';
import { Container, Paper, TextField, Button, Typography, Box, Alert, CircularProgress } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { authService } from '../Services/authService';

const ForgotPassword: React.FC = () => {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [emailError, setEmailError] = useState('');
  
  const navigate = useNavigate();

  const validateEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) return 'Email is required';
    if (!emailRegex.test(email)) return 'Invalid email format';
    return '';
  };

  const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setEmail(value);
    setEmailError(validateEmail(value));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const emailValidation = validateEmail(email);
    if (emailValidation) {
      setEmailError(emailValidation);
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await authService.forgotPassword(email);
      
      navigate('/verify-otp', {
        state: {
          email: email,
          otpToken: response.otpToken,
          type: 'forgot-password'
        }
      });
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to send reset email');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" align="center" gutterBottom>
          Forgot Password
        </Typography>
        
        <Typography variant="body1" align="center" sx={{ mb: 3, color: 'text.secondary' }}>
          Enter your email address and we'll send you a code to reset your password.
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Email"
            type="email"
            value={email}
            onChange={handleEmailChange}
            margin="normal"
            required
            error={!!emailError}
            helperText={emailError}
          />
          
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
            disabled={loading || !!emailError || !email}
          >
            {loading ? <CircularProgress size={24} /> : 'Send Reset Code'}
          </Button>
          
          <Button
            fullWidth
            variant="text"
            onClick={() => navigate('/login')}
          >
            Back to Login
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default ForgotPassword;
