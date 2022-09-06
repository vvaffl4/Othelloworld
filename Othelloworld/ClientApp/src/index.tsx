import { AuthProvider } from "oidc-react";
import React from "react";
import { createRoot } from 'react-dom/client';
import { Provider } from "react-redux";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import App from "./App";
import Game from "./components/Game";
import CreateGame from "./CreateGame";
import GameList from "./GameList";
import Home from "./Home";
import HomeContainer from "./HomeContainer";
import "./index.css";
import Play from "./Play";
import Register from "./Register";
import store from "./store";

createRoot(
  document.getElementById('root')!
)
  .render(
    <React.StrictMode>
      <Provider store={store}>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<App />}>
              <Route element={<HomeContainer />}>
                <Route index element={<Home />} />
                <Route path="register" element={<Register />} />
                <Route path="browse" element={<GameList />} />
              </Route>
              <Route path="play" element={<Play />}>
                <Route path=":gameToken" element={<Game />} />
                <Route index element={<CreateGame /> }/>
              </Route>
            </Route>
          </Routes>
        </BrowserRouter>
        </Provider>
    </React.StrictMode>
  );
 