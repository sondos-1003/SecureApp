import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Login from './components/Auth/Login';
import Register from './components/Auth/Register';

function App() {
    return (
        <Router>
            <div style={{ textAlign: 'center', padding: '2rem' }}>
                <h1>SecureApp</h1>
                <p>Welcome to SecureApp</p>
                <div style={{ marginTop: '1rem' }}>
                    <Link to="/login">
                        <button style={{ marginRight: '1rem' }}>LOGIN</button>
                    </Link>
                    <Link to="/register">
                        <button>REGISTER</button>
                    </Link>
                </div>

                <Routes>
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;