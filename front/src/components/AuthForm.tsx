import React, { useState } from 'react';
import axios from '../api';
import { TextField, Button, Container, Typography, Box, Grid } from '@mui/material';
import { User, CardDetails } from '../types';
import { useNavigate } from 'react-router-dom';
const AuthForm: React.FC = () => {
    const [isLogin, setIsLogin] = useState(true);
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [cardNumber, setCardNumber] = useState('');
    const [expiryDate, setExpiryDate] = useState('');
    const [cvv, setCvv] = useState('');
    const navigate = useNavigate();

    const handleBackClick = () => {
        navigate('/');
    };
    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();

        if (isLogin) {
            try {
                const response = await axios.post<User>('/Items/login', { username, email, password });
                console.log('Login successful', response.data);
               
                localStorage.setItem('user_id', response.data.userId.toString());
                handleBackClick();
            } catch (error) {
                console.error('Error logging in', error);
            }
        } else {
            try {
                const response = await axios.post<User>('/Items/register', { username, email, password });
                const userId = response.data.userId;
                console.log('Registration successful', response.data);
                localStorage.setItem('user_id', userId.toString());
                handleBackClick();
                const cardDetails: CardDetails = {
                    userId,
                    cardNumber,
                    cardholderName: username,
                    expiryDate,
                    cvv,
                };

                await axios.post('/Items/savecard', cardDetails);
                console.log('Card saved successfully');
            } catch (error) {
                console.error('Error registering', error);
            }
        }
    }; return (
        <Container component="main" maxWidth="xs">
            <Box
                sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Typography component="h1" variant="h5">
                    {isLogin ? 'Sign In' : 'Register'}
                </Typography>
                <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        label="Username"
                        autoComplete="username"
                        autoFocus
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        label="Email Address"
                        autoComplete="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        label="Password"
                        type="password"
                        autoComplete="current-password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    {!isLogin && (
                        <>
                            <TextField
                                margin="normal"
                                required
                                fullWidth
                                label="Card Number"
                                value={cardNumber}
                                onChange={(e) => setCardNumber(e.target.value)}
                            />
                            <Grid container spacing={2}>
                                <Grid item xs={6}>
                                    <TextField
                                        margin="normal"
                                        required
                                        fullWidth
                                        label="Expiry Date"
                                        placeholder="MM/YY"
                                        value={expiryDate}
                                        onChange={(e) => setExpiryDate(e.target.value)}
                                    />
                                </Grid>
                                <Grid item xs={6}>
                                    <TextField
                                        margin="normal"
                                        required
                                        fullWidth
                                        label="CVV"
                                        value={cvv}
                                        onChange={(e) => setCvv(e.target.value)}
                                    />
                                </Grid>
                            </Grid>
                        </>
                    )}
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 2 }}
                    >
                        {isLogin ? 'Sign In' : 'Register'}
                    </Button>
                    <Button
                        fullWidth
                        variant="text"
                        onClick={() => setIsLogin(!isLogin)}
                    >
                        {isLogin ? 'Switch to Register' : 'Switch to Sign In'}
                    </Button>
                </Box>
            </Box>
        </Container>
    );
};

export default AuthForm;

