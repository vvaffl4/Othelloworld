import { Button, List, ListItem, Popover, TextField } from '@mui/material';
import { pink } from '@mui/material/colors';
import { ChangeEvent, FC, useState } from 'react';
import { login } from '../store/Auth';
import { useAppDispatch } from '../store/Hooks';

const Login: FC = () => {
  const dispatch = useAppDispatch();

  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [email, setEmail] = useState<string>('');
  const [password, setPassword] = useState<string>('');

  const handleOpenLoginMenu = (event: React.MouseEvent<HTMLAnchorElement>) => {
    setAnchorEl(event.currentTarget);
  }

  const handleCloseLogoutMenu = () => {
    setAnchorEl(null);
  }

  const handleEmailChange = (event: ChangeEvent<HTMLInputElement>) =>
    setEmail(event.target.value);

  const handlePasswordChange = (event: ChangeEvent<HTMLInputElement>) =>
    setPassword(event.target.value);

  const handleLogin = () => {
    dispatch(login(
      { email, password },
      async result => {
        console.log(await result.json());
			}));
  }

  return (
    <>
      <Button
        component='a'
        onClick={handleOpenLoginMenu}
      >
        Login
      </Button>
      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        onClose={handleCloseLogoutMenu}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
      >
        <List
          // sx={{
          //   backgroundColor: pink[400]
          // }}        
        >
          <ListItem
            sx={{
              pb: 0
            }}
          >
            <TextField 
              variant="filled"
              size="small"
              label="username or e-mail"
              type="email"
              InputProps={{
                sx: {
                  '::before, :hover:not(.Mui-disabled)::before': {
                    borderBottom: 'none',
                  },
                  '::after': {
                    bottom: -2,
                    zIndex: 999
                  }
                }
              }}
              value={email}
              onChange={handleEmailChange}
            />
          </ListItem>
          <ListItem
            sx={{
              pt: 0
            }}
          >
            <TextField
              variant="filled"
              size="small"
              label="password"
              type="password"
              InputProps={{
                sx: {
                  borderRadius: '0 0 4px 4px',
                  '::before': {
                    top: 0,
                    borderTop: '1px solid #ffffffb3',
                    borderBottom: 'none'
                  },
                  '::after': {
                    top: 0,
                    borderTop: `2px solid ${pink[400]}`,
                    borderBottom: 'none'
                  },
                  ':hover:not(.Mui-disabled)::before': {
                    top: 0,
                    borderBottom: 'none',
                    borderTop: '1px solid #ffffffb3'
                  }
                }
              }}
              value={password}
              onChange={handlePasswordChange}
            />
          </ListItem>
          <ListItem>
            <Button
              size='small'
              onClick={handleLogin}>
              Log in
            </Button>
          </ListItem>
        </List>
      </Popover>
    </>
  );
}

export default Login;