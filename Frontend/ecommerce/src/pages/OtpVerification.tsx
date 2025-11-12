import React, { useEffect, useState } from 'react';
import { Container, Paper, TextField, Button, Typography, Box, Alert, CircularProgress } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import { authService } from '../Services/authService';

interface LocationState {
  email: string;
  otpToken: string;
  role?: number;
  formData?: any;
  type: 'registration' | 'forgot-password';
}

const OtpVerification: React.FC = () => {
  const [otp, setOtp] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [resendLoading, setResendLoading] = useState(false);
  const [success, setSuccess] = useState('');
  const [timer, setTimer] = useState(180);
  const [canResend, setCanResend] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();
  const { email, otpToken, role, formData, type } = location.state as LocationState || {};

  
  if (!email || !otpToken || !type) {
    navigate(type === 'forgot-password' ? '/forgot-password' : '/register');
    return null;
  }

  useEffect(() => {
    if (timer > 0) {
      const interval = setInterval(() => {
        setTimer(prev => prev - 1);
      }, 1000);
      return () => clearInterval(interval);
    } else {
      setCanResend(true);
    }
  }, [timer]);

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      if (type === 'registration') {
        // Registration OTP verification
        const response = await authService.verifyOtp(email, otp, otpToken, role!);
        const message = response.message || 'Registration completed successfully!';
        
        if (response.requiresApproval || response.RequiresApproval) {
          setSuccess(`${message} Your account is pending admin approval. You'll be notified once approved.`);
          setTimeout(() => {
            navigate('/login');
          }, 2000);
        } else if (response.requiresVendorProfile || response.RequiresVendorProfile){
          setSuccess(`${message} Redirecting to complete your vendor profile...`);
          setTimeout(() => {
            navigate('/vendor-profile-setup', {
              state: { userId: response.userId, email: response.email}
            });
          }, 2000);
        } else {
          setSuccess(`${message} Redirecting to login...`);
          setTimeout(() => {
            navigate('/login');
          }, 2000);
        }
      } else {
        // Forgot password OTP verification
        await authService.verifyForgotPasswordOtp(email, otp, otpToken);
        setSuccess('OTP verified successfully! Redirecting to reset password...');

        setTimeout(() => {
          navigate('/reset-password', {
            state: { email: email }
          });
        }, 2000);
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.description || 'OTP verification failed';
      
      if (errorMessage.includes('expired') || errorMessage.includes('invalid')) {
        setError(errorMessage + ' You can resend OTP after the timer expires.');
      } else {
        setError(errorMessage);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleResendOtp = async () => {
    setResendLoading(true);
    setError('');
    setSuccess('');

    try {
      if (type === 'registration' && formData) {
        // Resend registration OTP
        const response = await authService.register(formData);
        location.state.otpToken = response.otpToken;
      } else if (type === 'forgot-password') {
        // Resend forgot password OTP
        const response = await authService.forgotPassword(email);
        location.state.otpToken = response.otpToken;
      }

      setSuccess('New OTP sent to your email!');
      setTimer(180);
      setCanResend(false);
      setOtp('');
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to resend OTP');
    } finally {
      setResendLoading(false);
    }
  };

  // Dynamic content based on type
  const getTitle = () => {
    return type === 'registration' ? 'Verify Registration' : 'Verify Reset Code';
  };

  const getDescription = () => {
    return type === 'registration' 
      ? 'We\'ve sent a 6-digit verification code to' 
      : 'We\'ve sent a 6-digit reset code to';
  };

  const getBackRoute = () => {
    return type === 'registration' ? '/register' : '/forgot-password';
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" align="center" gutterBottom>
          {getTitle()}
        </Typography>

        <Typography variant="body1" align="center" sx={{ mb: 3 }}>
          {getDescription()} <strong>{email}</strong>
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {success}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Enter OTP"
            value={otp}
            onChange={(e) => setOtp(e.target.value.replace(/\D/g, ''))}
            margin="normal"
            required
            inputProps={{ maxLength: 6 }}
            placeholder="123456"
            disabled={loading || success.includes('successfully')}
          />

          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
            disabled={loading || otp.length !== 6 || success.includes('successfully')}
          >
            {loading ? <CircularProgress size={24} /> : 'Verify OTP'}
          </Button>

          <Box sx={{ textAlign: 'center', mb: 2 }}>
            {!canResend ? (
              <Typography variant="body2" color="text.secondary">
                Resend OTP in {formatTime(timer)}
              </Typography>
            ) : (
              <Button
                variant="outlined"
                onClick={handleResendOtp}
                disabled={resendLoading}
                fullWidth
              >
                {resendLoading ? <CircularProgress size={24} /> : 'Resend OTP'}
              </Button>
            )}
          </Box>

          <Button
            fullWidth
            variant="text"
            onClick={() => navigate(getBackRoute())}
            disabled={loading || resendLoading}
          >
            Back to {type === 'registration' ? 'Registration' : 'Email Entry'}
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default OtpVerification;
