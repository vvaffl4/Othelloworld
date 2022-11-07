import { Collapse, Divider, FormControlLabel, IconButton, ListItemButton, ListItemText, Menu, MenuItem, Switch, Tooltip, useTheme } from '@mui/material';
import { FC, useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { setPaletteMode } from '../store/UserInterface';
import { logout } from '../store/Auth';
import BadgeAvatar from './BadgeAvatar';
import { shallowEqual } from 'react-redux';
import { useNavigate } from 'react-router-dom';

const ProfileAvatar: FC = () => {
  const theme = useTheme();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const auth = useAppSelector(state => state.auth, shallowEqual);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);


  const handleOpenUserMenu = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  }

  const handleCloseUserMenu = () => {
    setAnchorEl(null);
  }

  const handleProfile = () => {
    navigate(`profile/${auth!.player!.username}`);
	}

  const handleChangePassword = () => {
    navigate('changepassword')
	}

  const handleLogout = () => {
    dispatch(logout());
  }

  const handlePaletteModeChange = (_: any, checked: boolean) => {
    dispatch(setPaletteMode(checked ? 'dark' : 'light'))

    theme.palette.mode = checked ? 'dark' : 'light';
  }

  return (
    <>
      <Tooltip title="Open settings">
        <IconButton
          onClick={handleOpenUserMenu}
          sx={{ p: 0 }}
        >
          <BadgeAvatar countryIso={auth.player!.countryCode.toLowerCase()} />
        </IconButton>
      </Tooltip >
      <Menu
        sx={{
          mt: '40px',
          zIndex: theme.zIndex.fab
        }}
        id="menu-appbar"
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        keepMounted
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        TransitionComponent={Collapse}
        TransitionProps={{
          timeout: {
            enter: 500,
            exit: 500
					}
				}}
        open={Boolean(anchorEl)}
        onClose={handleCloseUserMenu}
      >
        <MenuItem>
          <ListItemText>
            <FormControlLabel
              control={<Switch
                value={theme.palette.mode === 'dark'}
                onChange={handlePaletteModeChange}
              />}
              label="Dark Mode" />
          </ListItemText>
        </MenuItem>
        <MenuItem
          onClick={handleProfile}
        >
          <ListItemText
          >
            My Profile
          </ListItemText>
        </MenuItem>
        <Divider />
        <MenuItem
          onClick={handleChangePassword}
        >
          <ListItemText>
            Change Password
          </ListItemText>
        </MenuItem>
        <MenuItem>
          <ListItemText
            onClick={handleLogout}
          >
            Logout
          </ListItemText>
        </MenuItem>
      </Menu>
    </>
  );
}

export default ProfileAvatar;