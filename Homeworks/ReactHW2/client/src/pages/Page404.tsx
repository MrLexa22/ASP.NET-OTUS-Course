import { Container, Paper, Typography, Button, Box } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthProvider';

export default function Page404() {
   const navigate = useNavigate();

   const { user } = useAuth();
   return (
      <Container maxWidth="sm" sx={{ mt: 12 }}>
         <Paper sx={{ p: 4, textAlign: 'center' }}>
            <Typography variant="h3" gutterBottom>404</Typography>
            <Typography variant="h6" gutterBottom>Страница не найдена</Typography>
            <Typography color="text.secondary" sx={{ mb: 3 }}>
               Запрошенная страница не существует или была перемещена.
            </Typography>
            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center' }}>
               {user && <Button variant="contained" onClick={() => navigate('/home')}>На главную</Button>}
               {!user && <Button variant="outlined" onClick={() => navigate('/login')}>Войти</Button>}
            </Box>
         </Paper>
      </Container>
   );
}