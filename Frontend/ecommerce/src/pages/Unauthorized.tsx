import React from 'react';
import { Container, Typography, Button, Box } from '@mui/material';
import { useNavigate } from 'react-router-dom';

const Unauthorized: React.FC = () => {
  const navigate = useNavigate();

  return (
    <Container maxWidth="sm" sx={{ mt: 8, textAlign: 'center' }}>
      <Box>
        <Typography variant="h4" gutterBottom color="error">
          Access Denied
        </Typography>
        <Typography variant="body1" sx={{ mb: 3 }}>
          You don't have permission to access this page.
        </Typography>
        <Button 
          variant="contained" 
          onClick={() => navigate(-1)}
          sx={{ mr: 2 }}
        >
          Go Back
        </Button>
        <Button 
          variant="outlined" 
          onClick={() => navigate('/login')}
        >
          Login
        </Button>
      </Box>
    </Container>
  );
};

export default Unauthorized;