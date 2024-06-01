import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { Container, AppBar, Toolbar, Typography, Button } from '@mui/material';
import ProductGrid from './components/ProductGrid';
import AuthForm from './components/AuthForm';
import Cart from './components/Cart';

const App: React.FC = () => {
    return (
        <Router>
            <Container>
                <AppBar position="static">
                    <Toolbar>
                        <Typography variant="h6" sx={{ flexGrow: 1 }}>
                            My Shop
                        </Typography>
                        <Button color="inherit" component={Link} to="/">
                            Home
                        </Button>
                        <Button color="inherit" component={Link} to="/auth">
                            Auth
                        </Button>
                        <Button color="inherit" component={Link} to="/cart">
                            Cart
                        </Button>
                    </Toolbar>
                </AppBar>
                <Routes>
                    <Route path="/" element={<ProductGrid />} />
                    <Route path="/auth" element={<AuthForm />} />
                    <Route path="/cart" element={<Cart />} />
                </Routes>
            </Container>
        </Router>
    );
};

export default App;