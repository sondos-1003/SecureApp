import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Shared/Navbar';
import Home from './components/Home';
import Login from './components/Auth/Login';
import Register from './components/Auth/Register';
import ForgotPassword from './components/Auth/ForgotPassword';
import AdminPanel from './components/Admin/AdminPanel';
import UserDashboard from './components/User/UserDashboard';
import ProtectedRoute from './components/Shared/ProtectedRoute';
import NotFound from './components/Shared/NotFound';
import { Suspense } from 'react';
import { CircularProgress, Button } from '@mui/material';

function App() {
    return (
        <Router>
            <AuthProvider>
                <div style={{
                    textAlign: 'center',
                    padding: '2rem',
                    backgroundColor: '#e3f2fd'
                }}>
                    <h1>SecureApp</h1>
                    <p>Welcome to SecureApp</p>
                    <div>
                        {/* Changed buttons to Link components */}
                        <Button
                            component={Link}
                            to="/login"
                            variant="contained"
                            sx={{ margin: '0 1rem' }}
                        >
                            LOGIN
                        </Button>
                        <Button
                            component={Link}
                            to="/register"
                            variant="contained"
                        >
                            REGISTER
                        </Button>
                    </div>
                </div>

                <Navbar />
                <Suspense fallback={<div className="loader"><CircularProgress /></div>}>
                    <Routes>
                        <Route path="/" element={<Home />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="/forgot-password" element={<ForgotPassword />} />
                        <Route
                            path="/admin"
                            element={
                                <ProtectedRoute roles={['Admin']}>
                                    <AdminPanel />
                                </ProtectedRoute>
                            }
                        />
                        <Route
                            path="/user"
                            element={
                                <ProtectedRoute roles={['User', 'Admin']}>
                                    <UserDashboard />
                                </ProtectedRoute>
                            }
                        />
                        <Route path="*" element={<NotFound />} />
                    </Routes>
                </Suspense>
            </AuthProvider>
        </Router>
    );
}

export default App;