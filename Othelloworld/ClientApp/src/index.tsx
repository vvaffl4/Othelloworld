import React from "react";
import { createRoot } from 'react-dom/client';
import { Provider } from "react-redux";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import App from "./components/App";
import Game from "./components/Game";
import CreateGame from "./components/CreateGame";
import GameList from "./components/GameList";
import Home from "./components/Home";
import HomeContainer from "./components/HomeContainer";
import "./index.css";
import Play from "./components/Play";
import Register from "./components/Register";
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
                {/*<Route index element={<CreateGame /> }/>*/}
              </Route>
              {/*<Route path="play" element={<Play />}/>*/}
            </Route>
          </Routes>
        </BrowserRouter>
        </Provider>
    </React.StrictMode>
  );
 