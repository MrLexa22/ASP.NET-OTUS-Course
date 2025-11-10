import React, { useState } from 'react';
import { Container, Paper, Typography, TextField, Button, Box, Alert } from '@mui/material';
import { useAuth } from '../auth/AuthProvider';
import { useNavigate, Navigate } from 'react-router-dom';

export default function Login() {
   const [username, setUsername] = useState('');
   const [password, setPassword] = useState('');
   const [error, setError] = useState<string | null>(null);
   const [loading, setLoading] = useState(false);

   const { user, login } = useAuth();
   const navigate = useNavigate();

   if (user) return <Navigate to="/home" replace />;

   const handleSubmit = async (e: React.FormEvent) => {
      e.preventDefault();
      setError(null);
      setLoading(true);
      try {
         await login(username.trim(), password);
         navigate('/home', { replace: true });
         // eslint-disable-next-line @typescript-eslint/no-explicit-any
      } catch (err: any) {
         setError(err?.message || 'Ошибка входа');
      } finally {
         setLoading(false);
      }
   };

   return (
      <Container maxWidth="sm" sx={{ mt: 8 }}>
         <Paper sx={{ p: 4 }}>
            <Typography variant="h5" gutterBottom>Вход</Typography>

            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

            <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
               <TextField label="Имя пользователя" value={username} onChange={e => setUsername(e.target.value)} required />
               <TextField label="Пароль" type="password" value={password} onChange={e => setPassword(e.target.value)} required />
               <Button type="submit" variant="contained" disabled={loading}>
                  {loading ? 'Вход...' : 'Войти'}
               </Button>
            </Box>

            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 2 }}>
               <Typography variant="caption">Тестовый пользователь: admin / password</Typography>
               <Button variant="text" onClick={() => navigate('/register')}>Зарегистрироваться</Button>
            </Box>
         </Paper>
      </Container>
   );
}