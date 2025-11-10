import { Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Home from './pages/Home';
import Page404 from './pages/Page404';
import { withAuth, withoutAuth } from './hoc/withAuth';

const HomePage = withAuth(Home);
const LoginPage = withoutAuth(Login);
const RegisterPage = withoutAuth(Register);

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/home" element={<HomePage />} />
      <Route path="/" element={<Navigate to="/home" replace />} />
      <Route path="*" element={<Page404 />} />
    </Routes>
  );
}
