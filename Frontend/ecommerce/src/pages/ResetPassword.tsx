import React, { useState } from 'react';
import { Container, Paper, TextField, Button, Typography, Box, Alert, CircularProgress } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import { authService } from '../Services/authService';

interface LocationState {
  email: string;
}

const ResetPassword: React.FC = () => {
  const [formData, setFormData] = useState({
    newPassword: '',
    confirmPassword: ''
  });
  const [errors, setErrors] = useState({
    newPassword: '',
    confirmPassword: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const navigate = useNavigate();
  const location = useLocation();
  const { email } = location.state as LocationState || {};

  if (!email) {
    navigate('/forgot-password');
    return null;
  }

  const validatePassword = (password: string) => {
    if (!password) return 'Password is required';
    if (password.length < 8) return 'Password should be at least 8 characters';
    if (!/(?=.*[a-z])/.test(password)) return 'Password should contain at least one lowercase letter';
    if (!/(?=.*[A-Z])/.test(password)) return 'Password should contain at least one uppercase letter';
    if (!/(?=.*\d)/.test(password)) return 'Password should contain at least one number';
    return '';
  };

  const validateConfirmPassword = (confirmPassword: string, password: string) => {
    if (!confirmPassword) return 'Confirm password is required';
    if (confirmPassword !== password) return 'Passwords do not match';
    return '';
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    
    setFormData({
      ...formData,
      [name]: value
    });

    let fieldError = '';
    
    if (name === 'newPassword') {
      fieldError = validatePassword(value);
      // Also revalidate confirm password if it exists
      if (formData.confirmPassword) {
        setErrors(prev => ({
          ...prev,
          confirmPassword: validateConfirmPassword(formData.confirmPassword, value)
        }));
      }
    } else if (name === 'confirmPassword') {
      fieldError = validateConfirmPassword(value, formData.newPassword);
    }

    setErrors({
      ...errors,
      [name]: fieldError
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await authService.resetPassword(email, formData.newPassword,formData.confirmPassword);
      alert('Password reset successfully! Please login with your new password.');
      navigate('/login');
    } catch (err: any) {
      setError(err.response?.data?.description || 'Password reset failed');
    } finally {
      setLoading(false);
    }
  };

  const isFormValid = () => {
    return formData.newPassword && formData.confirmPassword && 
           !errors.newPassword && !errors.confirmPassword;
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" align="center" gutterBottom>
          Reset Password
        </Typography>

        <Typography variant="body1" align="center" sx={{ mb: 3, color: 'text.secondary' }}>
          Enter your new password for <strong>{email}</strong>
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="New Password"
            type="password"
            name="newPassword"
            value={formData.newPassword}
            onChange={handleChange}
            margin="normal"
            required
            error={!!errors.newPassword}
            helperText={errors.newPassword}
          />

          <TextField
            fullWidth
            label="Confirm New Password"
            type="password"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleChange}
            margin="normal"
            required
            error={!!errors.confirmPassword}
            helperText={errors.confirmPassword}
          />

          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
            disabled={loading || !isFormValid()}
          >
            {loading ? <CircularProgress size={24} /> : 'Reset Password'}
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

export default ResetPassword;
