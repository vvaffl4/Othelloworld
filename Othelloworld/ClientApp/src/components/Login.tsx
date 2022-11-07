import { Button, LinearProgress, Link, List, ListItem, Popover, Slide, TextField } from '@mui/material';
import { pink } from '@mui/material/colors';
import { FC, useState } from 'react';
import { login } from '../store/Auth';
import { useAppDispatch } from '../store/Hooks';

interface FormProps {
  email: string;
  password: string;
}

const Login: FC = () => {
  const dispatch = useAppDispatch();

  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [processing, isProcessing] = useState(false);


  const [form, setForm] = useState<FormProps>({
    email: '',
    password: ''
  });
  const [errors, setErrors] = useState<Partial<FormProps>>({
    email: '',
    password: ''
  });

  const handleOpenLoginMenu = (event: React.MouseEvent<HTMLAnchorElement>) => {
    setAnchorEl(event.currentTarget);
  }

  const handleCloseLogoutMenu = () => {
    setAnchorEl(null);
  }

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setForm({
      ...form,
      [event.target.name]: event.target.value
    });
  }

  const handleLogin = () => {
    isProcessing(true);
    dispatch(login(
      form,
      handleInvalidInput
    ));
  }

  const handleInvalidInput = async (result) => {
    isProcessing(false);
    const jsonResult = await result.json();

    if (jsonResult) {
      const errors = Object.entries(jsonResult.errors)
        .reduce((state, [key, value]) => ({
          ...state,
          [key.toLowerCase()]: (value as string[]).join('\n')
        }), {});

      setErrors(errors);
		}
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
        sx={{
          zIndex: (theme) => theme.zIndex.appBar - 1
        }}
        TransitionComponent={
          Slide
				}
      >
        <List>
          <ListItem
            sx={{
              pb: 0
            }}
          >
            <TextField 
              variant="filled"
              size="small"
              label="E-mail"
              type="email"
              name="email"
              disabled={processing}
              error={errors.email !== ''}
              helperText={errors.email}
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
              value={form.email}
              onChange={handleChange}
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
              name="password"
              disabled={processing}
              error={errors.password !== ''}
              helperText={errors.password}
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
              value={form.password}
              onChange={handleChange}
            />
          </ListItem>
          <ListItem>
            <Button
              size='small'
              disabled={processing}
              onClick={handleLogin}
            >
              Log in
            </Button>
          </ListItem>
          <ListItem>
            <Link href="forgotpassword">Forgot Password?</Link>
          </ListItem>
        </List>
        {processing && (<LinearProgress />)}
      </Popover>
    </>
  );
}

export default Login;