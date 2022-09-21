import { Button, CssBaseline, ThemeProvider, useMediaQuery } from "@mui/material";
import React, { useEffect, useMemo } from "react";
import { useDispatch } from "react-redux";
import { Outlet } from "react-router-dom";
import Nav from "./Nav";
import RGlobe from "./RGlobe";
import { useAppSelector } from "../store/Hooks";
import { setPaletteMode } from "../store/UserInterface";
import createTheme from "../Theme";
import { useSnackbar } from "notistack";

const App = () => {
  const { enqueueSnackbar, closeSnackbar } = useSnackbar(); 
  const dispatch = useDispatch();
  const darkMode = useMediaQuery('(prefers-color-scheme: dark)');
  const isDarkModeEnabled = useAppSelector(state => state.ui.mode);

  const challengeActions = snackbarId => (
    <>
      <Button onClick={() => { closeSnackbar(snackbarId) }}>
        Accept
      </Button>
      <Button onClick={() => { closeSnackbar(snackbarId) }}>
        Decline
      </Button>
    </>
  );

  const errorActions = snackbarId => (
    <>
      <Button
        color="inherit"
        onClick={() => { closeSnackbar(snackbarId) }}
      >
        Close
      </Button>
    </>
  );

  useEffect(() => {
    dispatch(setPaletteMode(darkMode ? 'dark' : 'light'));

    enqueueSnackbar("You've gotten a challenge from... ", { persist: true, action: challengeActions });
    enqueueSnackbar("Error, error, error ", { variant: 'error', action: errorActions });

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
        {/*<Challenge />*/}
        {/*<ErrorMessage/>*/}
      </div>
    </ThemeProvider>
  );
};

export default App; 