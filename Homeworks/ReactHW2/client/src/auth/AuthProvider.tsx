import React, { createContext, useContext, useEffect, useState } from 'react';

type User = { name: string } | null;

type AuthContextType = {
   user: User;
   login: (username: string, password: string) => Promise<void>;
   logout: () => void;
   register: (username: string, password: string) => Promise<void>;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
   const [user, setUser] = useState<User>(null);

   useEffect(() => {
      const storedUsers = localStorage.getItem('users');
      if (!storedUsers) {
         const defaultUsers = [{ username: 'admin', password: 'password' }];
         localStorage.setItem('users', JSON.stringify(defaultUsers));
      }

      const stored = localStorage.getItem('authUser');
      if (stored) setUser(JSON.parse(stored));
   }, []);

   const login = async (username: string, password: string) => {
      await new Promise(res => setTimeout(res, 300)); // имитация задержки

      const usersRaw = localStorage.getItem('users') || '[]';
      const users: Array<{ username: string; password: string }> = JSON.parse(usersRaw);

      const found = users.find(u => u.username === username && u.password === password);
      if (found) {
         const authUser = { name: username };
         localStorage.setItem('authUser', JSON.stringify(authUser));
         setUser(authUser);
         return;
      }

      throw new Error('Неверное имя пользователя или пароль');
   };

   const register = async (username: string, password: string) => {
      await new Promise(res => setTimeout(res, 800)); // имитация задержки

      const uname = username.trim();
      if (!uname) throw new Error('Имя пользователя не может быть пустым');
      if (password.length < 4) throw new Error('Пароль должен быть не короче 4 символов');

      const usersRaw = localStorage.getItem('users') || '[]';
      const users: Array<{ username: string; password: string }> = JSON.parse(usersRaw);

      if (users.some(u => u.username === uname)) {
         throw new Error('Пользователь с таким именем уже зарегистрирован');
      }

      const newUser = { username: uname, password };
      users.push(newUser);
      localStorage.setItem('users', JSON.stringify(users));

      // автоматически логиним нового пользователя
      const authUser = { name: uname };
      localStorage.setItem('authUser', JSON.stringify(authUser));
      setUser(authUser);
   };

   const logout = () => {
      localStorage.removeItem('authUser');
      setUser(null);
   };

   return (
      <AuthContext.Provider value={{ user, login, logout, register }}>
         {children}
      </AuthContext.Provider>
   );
};

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
   const ctx = useContext(AuthContext);
   if (!ctx) throw new Error('useAuth must be used within AuthProvider');
   return ctx;
}