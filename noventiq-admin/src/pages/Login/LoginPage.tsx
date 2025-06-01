import React, { useState } from 'react';
import {
    Box,
    Button,
    Container,
    TextField,
    Typography,
    Paper,
    Alert,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../stores/auth.store';

export const LoginPage: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();
    const { login, isLoading, error } = useAuthStore();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        await login({ email, password });
        if (!error) {
            navigate('/users');
        }
    };

    return (
        <Container component="main" maxWidth="xs">
            <Box
                sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Paper
                    elevation={3}
                    sx={{
                        padding: 4,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        width: '100%',
                    }}
                >
                    <Typography component="h1" variant="h5">
                        Sign in
                    </Typography>
                    {error && (
                        <Alert severity="error" sx={{ mt: 2, width: '100%' }}>
                            {error}
                        </Alert>
                    )}
                    <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            id="email"
                            label="Email Address"
                            name="email"
                            autoComplete="email"
                            autoFocus
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            label="Password"
                            type="password"
                            id="password"
                            autoComplete="current-password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            disabled={isLoading}
                        >
                            {isLoading ? 'Signing in...' : 'Sign In'}
                        </Button>
                    </Box>
                </Paper>
            </Box>
        </Container>
    );
}; 