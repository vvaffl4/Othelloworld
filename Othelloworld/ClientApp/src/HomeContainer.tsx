import { Button, Checkbox, Container, Divider, FormControlLabel, Grid, Paper, TextField, Typography, useTheme } from '@mui/material';
import { PerspectiveCamera } from '@react-three/drei';
import { AuthProvider } from 'oidc-react';
import { FC } from 'react';
import { Outlet } from 'react-router-dom';

const oidcConfig = {
  onSignIn: () => {
    // Redirect?
  },
  authority: "https://localhost:44367",
  clientId: "ReactAspAuth",
  //redirectUri: "https://localhost:44367",
  redirectUri: "https://localhost:44367/authentication/login-callback",
  postLogoutRedirectUri: "https://localhost:44367/authentication/logout-callback",
  responseType: "code",
  scope: "OthelloworldAPI openid profile"
};

const HomeContainer: FC = () => {
  return (
    <AuthProvider {...oidcConfig}>
    <div
      style={{
        position: 'absolute',
        top: 100,
        left: 0,
        right: 0
      }}
    >
      <Outlet />
      </div>
    </AuthProvider>
  )
}

export default HomeContainer;