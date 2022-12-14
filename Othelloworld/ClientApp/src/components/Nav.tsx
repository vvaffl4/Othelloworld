import Brightness1OutlinedIcon from '@mui/icons-material/Brightness1Outlined';
import Brightness2Icon from '@mui/icons-material/Brightness2';
import { AppBar, Button, ButtonGroup, Toolbar, Typography, useTheme } from "@mui/material";
import { FC, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Login from "./Login";
import ProfileAvatar from "./ProfileAvatar";
import { useAppSelector } from "../store/Hooks";

const Nav: FC = () => {
  const navigate = useNavigate();
  const authenticated = useAppSelector(state => state.auth.authenticated);

  //useEffect(() => {
  //  if (!authenticated) {
  //    navigate('/');
  //  }
  //}, [authenticated])

  return (
    <AppBar>
      <Toolbar sx={{ minHeight: '56px !important' }}>
        { authenticated && (
          <div
            style={{
              height: '100%',
              overflow: 'hidden'
            }}
          >
            <Button 
              variant="contained"
              disableElevation 
              sx={{
                mr: 1.5,
                my: -1.5,
                width: '80px',
                height: '80px',
                borderRadius: '50%',
                fontFamily: 'monospace',
                fontWeight: 700,
                fontSize: 22,
              }}
              href="play"
            >
              Play
            </Button>
          </div>
        ) }
        <Button
          variant="contained"
          disableElevation
          href="/"
        >
          <Brightness1OutlinedIcon sx={{ display: { xs: 'none', md: 'flex' } }} />
          <Brightness2Icon sx={{ display: { xs: 'none', md: 'flex' }, ml: -1.5 }} />
          <Typography
            variant="h5"
            noWrap
            sx={{
              ml: 2,
              display: { xs: 'none', md: 'flex' },
              fontFamily: 'monospace',
              fontWeight: 700,
              letterSpacing: '.3rem',
              color: 'inherit',
              textDecoration: 'none',
            }}
          >
            OTHELLO
          </Typography>
          <Typography
            component={'sup'}
          >
            WORLD
          </Typography>
        </Button>

        <div style={{ flexGrow: 1, display: 'flex' }}>
          {/* {pages.map((page) => (
            <Button
              key={page}
              onClick={handleCloseNavMenu}
              sx={{ my: 2, color: 'white', display: 'block' }}
            >
              {page}
            </Button>
          ))} */}
        </div>
        <div style={{ flexGrow: 0 }}>
          { authenticated ? (
            <ProfileAvatar/>
          ) : (
            <ButtonGroup 
              variant="contained" 
              aria-label="medium secondary button group"
              disableElevation
            >
              <Login/>
              <Button
                href="register"
              >
                Register
              </Button>
            </ButtonGroup>
          ) }
        </div>
      </Toolbar>
    </AppBar>
  )
}

export default Nav;