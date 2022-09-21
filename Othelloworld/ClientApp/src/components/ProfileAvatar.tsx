﻿import { styled } from '@mui/material/styles';
import { Avatar, Badge, Collapse, Divider, FormControlLabel, IconButton, ListItemButton, ListItemText, Menu, MenuItem, Switch, Theme, Tooltip, useTheme } from '@mui/material';
import { FC, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { setPaletteMode } from '../store/UserInterface';
import { logout } from '../store/Auth';
import BadgeAvatar from './BadgeAvatar';
import { shallowEqual } from 'react-redux';

const ProfileAvatar: FC = () => {
  const theme = useTheme();
  const dispatch = useAppDispatch();
  const auth = useAppSelector(state => state.auth, shallowEqual);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const handleOpenUserMenu = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  }

  const handleCloseUserMenu = () => {
    setAnchorEl(null);
  }

  const handleLogout = () => {
    dispatch(logout());
  }

  const handlePaletteModeChange = (_: any, checked: boolean) => {
    dispatch(setPaletteMode(checked ? 'dark' : 'light'))
  }

  return (
    <>
      <Tooltip title="Open settings">
        <IconButton
          onClick={handleOpenUserMenu}
          sx={{ p: 0 }}
        >
          <BadgeAvatar countryIso="nl" />
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
        <MenuItem>
          <ListItemButton
            href={`profile/${auth?.username}`}
          >
            My Profile
          </ListItemButton>
        </MenuItem>
        <Divider />
        <MenuItem>
          <ListItemButton
            onClick={handleLogout}
          >
            Logout
          </ListItemButton>
        </MenuItem>
      </Menu>
    </>
  );
}

export default ProfileAvatar;