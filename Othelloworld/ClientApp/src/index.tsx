import React from "react";
import { createRoot } from 'react-dom/client';
import { Provider } from "react-redux";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import App from "./components/App";
import Game from "./components/Game";
import CreateGame from "./components/CreateGame";
import GameList from "./components/GameList";
import Home from "./components/Home";
import Profile from "./components/Profile";
import HomeContainer from "./components/HomeContainer";
import "./index.css";
import Play from "./components/Play";
import Register from "./components/Register";
import ChangePassword from "./components/ChangePassword";
import store from "./store";
import { SnackbarProvider } from "notistack";
import ForgotPassword from "./components/ForgotPassword";
import RecoverPassword from "./components/RecoverPassword";

createRoot(
  document.getElementById('root')!
)
  .render(
    <React.StrictMode>
      <Provider store={store}>
        <SnackbarProvider maxSnack={3}
          preventDuplicate
          autoHideDuration={6000}
        >
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<App />}>
                <Route element={<HomeContainer />}>
                  <Route index element={<Home />} />
                  <Route path="register" element={<Register />} />
                  <Route path="changepassword" element={<ChangePassword />} />
                  <Route path="forgotpassword" element={<ForgotPassword />} />
                  <Route path="recoverpassword/:email/:resetToken" element={<RecoverPassword />} />
                  <Route path="browse" element={<GameList />} />
                </Route>
                <Route path="play">
                  <Route path="new" element={<CreateGame />} />
                  <Route path="browse" element={<GameList />} />
                  <Route path="" element={<Play />} />
                </Route> 
                <Route path="profile/:username" element={<Profile />} />
              </Route>
            </Routes>
            </BrowserRouter>
          </SnackbarProvider>
        </Provider>
    </React.StrictMode>
  );
 