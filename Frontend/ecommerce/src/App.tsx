import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/authContext';
import Navbar from './componenets/navBar';
import ProtectedRoute from './componenets/ProtectedRoute';
import Login from './pages/Login';
import Register from './pages/Register';
import CustomerDashboard from './pages/CustomerDashboard';
import ProductDetail from './pages/ProductDetail';
import Cart from './pages/Cart';
import AdminDashboard from './pages/AdminDashboard';
import VendorDashboard from './pages/VendorDashboard';
import OtpVerification from './pages/OtpVerification';
import ForgotPassword from './pages/ForgotPassword';
import ResetPassword from './pages/ResetPassword';
import VendorProfileSetup from './pages/VendorProfileSetup';
import Checkout from './pages/Checkout';
import Payment from './pages/Payment';
import Orders from './pages/Orders';
import OrderHistory from './pages/OrderHistory';
import Unauthorized from './pages/Unauthorized';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Navbar />
        <Routes>
          <Route path="/" element={<Navigate to="/login" />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/verify-otp" element={<OtpVerification />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          <Route path="/unauthorized" element={<Unauthorized />} />
          
          <Route path="/admin-dashboard" element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <AdminDashboard />
            </ProtectedRoute>
          } />
          
          <Route path="/vendor-dashboard" element={
            <ProtectedRoute allowedRoles={['Vendor']}>
              <VendorDashboard />
            </ProtectedRoute>
          } />
          
          <Route path="/vendor-profile-setup" element={<VendorProfileSetup />} />
          
          <Route path="/dashboard" element={
            <ProtectedRoute allowedRoles={['Customer']}>
              <CustomerDashboard />
            </ProtectedRoute>
          } />
          
          <Route path="/customer-dashboard" element={
            <ProtectedRoute allowedRoles={['Customer']}>
              <CustomerDashboard />
            </ProtectedRoute>
          } />
          
          <Route path="/product/:id" element={
            <ProtectedRoute allowedRoles={['Customer']}>
              <ProductDetail />
            </ProtectedRoute>
          } />
          
          <Route path="/cart" element={
            <ProtectedRoute allowedRoles={['Customer']}>
              <Cart />
            </ProtectedRoute>
          } />
          
          <Route path="/checkout" element={
            <ProtectedRoute allowedRoles={['Customer']}>
              <Checkout />
            </ProtectedRoute>
          } />
          
          <Route path="/payment" element={
            <ProtectedRoute allowedRoles={['Customer']}>
              <Payment />
            </ProtectedRoute>
          } />
          
          <Route path="/orders" element={
            <ProtectedRoute allowedRoles={['Customer']}>
              <Orders />
            </ProtectedRoute>
          } />
          
          <Route path="/order-history" element={
            <ProtectedRoute allowedRoles={['Customer']}>
              <OrderHistory />
            </ProtectedRoute>
          } />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
