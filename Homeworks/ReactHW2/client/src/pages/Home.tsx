import { AppBar, Toolbar, Typography, Button, Container, Box } from '@mui/material';
import { useAuth } from '../auth/AuthProvider';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { increment, decrement, addBy, reset } from '../store/slices/counterSlice';

export default function Home() {
   const { user, logout } = useAuth();
   const navigate = useNavigate();

   const handleLogout = () => {
      logout();
      navigate('/login', { replace: true });
   };

   const count = useAppSelector(state => state.counter.value);
   const dispatch = useAppDispatch();

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

            <Box sx={{ mt: 4, display: 'flex', gap: 2, alignItems: 'center' }}>
               <Button variant="outlined" onClick={() => dispatch(decrement())}>-</Button>
               <Typography variant="h6" sx={{ minWidth: 48, textAlign: 'center' }}>{count}</Typography>
               <Button variant="outlined" onClick={() => dispatch(increment())}>+</Button>
               <Button variant="text" onClick={() => dispatch(addBy(5))}>+5</Button>
               <Button variant="contained" color="secondary" onClick={() => dispatch(reset())}>Сброс</Button>
            </Box>
         </Container>
      </>
   );
}