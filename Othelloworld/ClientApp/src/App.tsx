import { CssBaseline, ThemeProvider, useMediaQuery } from "@mui/material";
import React, { useEffect, useMemo } from "react";
import { useDispatch } from "react-redux";
import { Outlet } from "react-router-dom";
import Nav from "./Nav";
import RGlobe from "./RGlobe";
import { useAppSelector } from "./store/Hooks";
import { setPaletteMode } from "./store/UserInterface";
import createTheme from "./Theme";

const App = () => {
  const dispatch = useDispatch();
  const darkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const isDarkModeEnabled = useAppSelector(state => state.ui.mode);

  useEffect(() => {
    dispatch(setPaletteMode(darkMode ? 'dark' : 'light'));
  }, []);

  const theme = useMemo(() =>
    createTheme(isDarkModeEnabled ? 'dark' : 'light')
    , [isDarkModeEnabled])


  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Nav/>
      <div
        style={{
          position: 'relative',
          paddingTop: '0px',
        }}
      >
        <RGlobe />
        <Outlet />
      </div>
    </ThemeProvider>
  );
};

export default App; 