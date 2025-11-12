import React, { useState } from 'react';
import {
  Container,
  Paper,
  TextField,
  Button,
  Typography,
  Box,
  Alert,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { authService } from '../Services/authService';

const Register: React.FC = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    firstName: '',
    middleName: '',
    lastName: '',
    role: 1
  });
  
  const [errors, setErrors] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    firstName: '',
    middleName: '',
    lastName: ''
  });
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const navigate = useNavigate();

  // Validation functions
  const validateEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) return 'Email is required';
    if (!emailRegex.test(email)) return 'Invalid email format';
    return '';
  };

  const validateName = (name: string, fieldName: string, required = true) => {
    const nameRegex = /^[A-Za-z\s]+$/;
    if (required && !name) return `${fieldName} is required`;
    if (name && !nameRegex.test(name)) return `${fieldName} should contain only letters`;
    if (name && name.length < 2) return `${fieldName} should be at least 2 characters`;
    return '';
  };

  const validatePhone = (phone: string) => {
    const phoneRegex = /^[6-9]\d{9}$/;
    if (!phone) return 'Phone number is required';
    if (!phoneRegex.test(phone)) return 'Invalid phone number (should be 10 digits starting with 6-9)';
    return '';
  };

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
    
    // Handle special input restrictions
    let filteredValue = value;
    
    // Only letters and spaces for name fields
    if (['firstName', 'middleName', 'lastName'].includes(name)) {
      filteredValue = value.replace(/[^A-Za-z\s]/g, '');
    }
    
    // Only numbers for phone
    if (name === 'phone') {
      filteredValue = value.replace(/\D/g, '').slice(0, 10);
    }

    setFormData({
      ...formData,
      [name]: filteredValue
    });

    // Real-time validation
    let fieldError = '';
    
    switch (name) {
      case 'email':
        fieldError = validateEmail(filteredValue);
        break;
      case 'firstName':
        fieldError = validateName(filteredValue, 'First name', true);
        break;
      case 'middleName':
        fieldError = validateName(filteredValue, 'Middle name', false);
        break;
      case 'lastName':
        fieldError = validateName(filteredValue, 'Last name', true);
        break;
      case 'phone':
        fieldError = validatePhone(filteredValue);
        break;
      case 'password':
        fieldError = validatePassword(filteredValue);
        // Also revalidate confirm password if it exists
        if (formData.confirmPassword) {
          setErrors(prev => ({
            ...prev,
            confirmPassword: validateConfirmPassword(formData.confirmPassword, filteredValue)
          }));
        }
        break;
      case 'confirmPassword':
        fieldError = validateConfirmPassword(filteredValue, formData.password);
        break;
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
      const { confirmPassword, ...apiData } = formData;
      const response = await authService.register(apiData);

      navigate('/verify-otp', {
        state: {
          email: formData.email,
          otpToken: response.otpToken,
          role: formData.role,
          formData: apiData,
          type : 'registration'
        }
      });
    } catch (err: any) {
      setError(err.response?.data?.description || 'Registration failed');
    } finally {
      setLoading(false);
    }
  };

  const isFormValid = () => {
    const hasNoErrors = Object.values(errors).every(error => error === '');
    const hasAllRequiredFields = formData.firstName && formData.lastName && 
                                formData.email && formData.phone && 
                                formData.password && formData.confirmPassword;
    return hasNoErrors && hasAllRequiredFields;
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" align="center" gutterBottom>
          Register
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="First Name"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            margin="normal"
            required
            error={!!errors.firstName}
            helperText={errors.firstName}
          />

          <TextField
            fullWidth
            label="Middle Name (Optional)"
            name="middleName"
            value={formData.middleName}
            onChange={handleChange}
            margin="normal"
            error={!!errors.middleName}
            helperText={errors.middleName}
          />

          <TextField
            fullWidth
            label="Last Name"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            margin="normal"
            required
            error={!!errors.lastName}
            helperText={errors.lastName}
          />

          <TextField
            fullWidth
            label="Email"
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            margin="normal"
            required
            error={!!errors.email}
            helperText={errors.email}
          />

          <TextField
            fullWidth
            label="Phone"
            name="phone"
            value={formData.phone}
            onChange={handleChange}
            margin="normal"
            required
            error={!!errors.phone}
            helperText={errors.phone}
            placeholder="9876543210"
          />

          <TextField
            fullWidth
            label="Password"
            type="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            margin="normal"
            required
            error={!!errors.password}
            helperText={errors.password}
          />

          <TextField
            fullWidth
            label="Confirm Password"
            type="password"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleChange}
            margin="normal"
            required
            error={!!errors.confirmPassword}
            helperText={errors.confirmPassword}
          />

          <FormControl fullWidth margin="normal">
            <InputLabel>Role</InputLabel>
            <Select
              value={formData.role}
              label="Role"
              onChange={(e) => setFormData({ ...formData, role: e.target.value as number })}
            >
              <MenuItem value={1}>Customer</MenuItem>
              <MenuItem value={2}>Vendor</MenuItem>
            </Select>
          </FormControl>

          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
            disabled={loading || !isFormValid()}
          >
            {loading ? <CircularProgress size={24} /> : 'Register'}
          </Button>

          <Button
            fullWidth
            variant="text"
            onClick={() => navigate('/login')}
          >
            Already have an account? Login
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default Register;
