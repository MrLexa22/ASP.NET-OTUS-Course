import React, { useState } from 'react';
import { Container, Paper, Typography, TextField, Button, Box, Alert } from '@mui/material';
import { useAuth } from '../auth/AuthProvider';
import { useNavigate, Navigate } from 'react-router-dom';

export default function Register() {
   const { user, register } = useAuth();
   const navigate = useNavigate();

   const [username, setUsername] = useState('');
   const [password, setPassword] = useState('');
   const [confirm, setConfirm] = useState('');
   const [error, setError] = useState<string | null>(null);
   const [loading, setLoading] = useState(false);

   if (user) return <Navigate to="/home" replace />;

   const handleSubmit = async (e: React.FormEvent) => {
      e.preventDefault();
      setError(null);

      if (password !== confirm) {
         setError('Пароли не совпадают');
         return;
      }

      setLoading(true);
      try {
         await register(username.trim(), password);
         navigate('/home', { replace: true });
         // eslint-disable-next-line @typescript-eslint/no-explicit-any
      } catch (err: any) {
         setError(err?.message || 'Ошибка регистрации');
      } finally {
         setLoading(false);
      }
   };

   return (
      <Container maxWidth="sm" sx={{ mt: 8 }}>
         <Paper sx={{ p: 4 }}>
            <Typography variant="h5" gutterBottom>Регистрация</Typography>

            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

            <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
               <TextField label="Имя пользователя" value={username} onChange={e => setUsername(e.target.value)} required />
               <TextField label="Пароль" type="password" value={password} onChange={e => setPassword(e.target.value)} required />
               <TextField label="Подтверждение пароля" type="password" value={confirm} onChange={e => setConfirm(e.target.value)} required />
               <Button type="submit" variant="contained" disabled={loading}>
                  {loading ? 'Регистрируем...' : 'Зарегистрироваться'}
               </Button>
            </Box>

            <Typography variant="caption" display="block" sx={{ mt: 2 }}>
               Пароль минимум 4 символа. Тестовый пользователь: admin / password
            </Typography>
         </Paper>
      </Container>
   );
}