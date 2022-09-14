import { FC } from 'react';
import { Navigate, Outlet } from 'react-router-dom';

interface AuthOutletProps {
  auth: boolean
}

const AuthOutlet: FC<AuthOutletProps> = ({ auth }) => {
  return auth ? <Outlet /> : <Navigate to="/" replace={true} />;
};

export default AuthOutlet;