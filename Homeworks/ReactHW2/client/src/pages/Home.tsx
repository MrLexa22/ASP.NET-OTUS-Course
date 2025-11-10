import { AppBar, Toolbar, Typography, Button, Container } from '@mui/material';
import { useAuth } from '../auth/AuthProvider';
import { useNavigate } from 'react-router-dom';

export default function Home() {
   const { user, logout } = useAuth();
   const navigate = useNavigate();

   const handleLogout = () => {
      logout();
      navigate('/login', { replace: true });
   };

   return (
      <>
         <AppBar position="static">
            <Toolbar>
               <Typography variant="h6" sx={{ flexGrow: 1 }}>Домашняя</Typography>
               <Button color="inherit" onClick={handleLogout}>Выйти</Button>
            </Toolbar>
         </AppBar>
         <Container sx={{ mt: 4 }}>
            <Typography variant="h4">Привет, {user?.name}</Typography>
            <Typography sx={{ mt: 2 }}>Это защищённая домашняя страница.</Typography>
         </Container>
      </>
   );
}