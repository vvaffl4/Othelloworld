import { Box, Button, Container, Divider, Paper, Stack, TextField, Typography, useTheme } from "@mui/material";
import { FC, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { changePassword, recoverPassword } from "../store/Auth";
import { useAppDispatch, useAppSelector } from "../store/Hooks";
import * as React from 'react';
import ReCAPTCHAElement, { ReCAPTCHA } from "react-google-recaptcha"

interface FormProps {
  newPassword: string;
  validation: string;
}

const RecoverPassword: FC = () => {
  const theme = useTheme();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const captchaRef = useRef<ReCAPTCHA>(!null)

  let { email, resetToken } = useParams();
  const [processing, isProcessing] = useState(false);
  const auth = useAppSelector(state => state.auth);

  const [form, setForm] = useState<FormProps>({
    newPassword: '',
    validation: ''
  });
  const [errors, setErrors] = useState<Partial<FormProps>>({
    newPassword: '',
    validation: ''
  });

  useEffect(() => {
    if (auth.authenticated) {
      isProcessing(false);
      navigate(`/profile/${auth.player!.username}`, { replace: true });
    }
  }, [auth.authenticated]);

  useEffect(() => {
    if (
      !email || email === '' ||
      !resetToken || resetToken === '') {
      navigate('/');
		}
  }, [email, resetToken]);

  const handleChange = (event: React.FocusEvent<HTMLInputElement>) => {
    console.log(event.target.value);

    setForm({
      ...form,
      [event.target.name]: event.target.value
    });
  }

  const handleSubmit = () => {
    if (form.newPassword !== form.validation) {
      setErrors({ ...errors, validation: "Validation password does not match new password" });
      return;
    }

    isProcessing(true);

    const token = captchaRef.current.getValue();

    dispatch(recoverPassword(
      {
        ...form, email: decodeURIComponent(email!), resetToken: decodeURIComponent(resetToken!)
      },
      token,
      async (result) => {
        const errors = Object.entries((await result.json()).errors)
          .reduce((state, [key, value]) => ({
            ...state,
            [key.toLowerCase()]: (value as string[]).join('\n')
          }), {});

        isProcessing(false);
        setErrors(errors);
      })
    );
  }

  return (
    <Container
      sx={{
        position: 'absolute',
        top: '600px',
        left: 0,
        right: 0
      }}
    >
      <Paper sx={{
        overflow: 'hidden',
        backgroundColor: 'neutral'
      }}>
        <form>
          <Box
            component="div"
            sx={{
              p: 4,
              backgroundColor: '#000000',
              borderBottom: '1px solid #ffffff17'
            }}
          >
            <Typography
              variant="h3"
              color='white'
            >
              Password Recovery
            </Typography>
          </Box>
          <Stack
            direction="row"
            divider={<Divider
              orientation="vertical"
              variant="middle"
              flexItem
            />}
            spacing={2}
            sx={{
              p: 4
            }}
          >
            <Box
              component="div"
              sx={{
                flex: 1
              }}
            >
              <TextField
                required
                fullWidth
                id="newPassword"
                name="newPassword"
                label="New password"
                type="password"
                variant="standard"
                error={errors.newPassword !== undefined && errors.newPassword !== ''}
                helperText={errors.newPassword}
                disabled={processing}
                onBlur={handleChange}
              />
              <Typography variant="caption">
                The given password must have the following criteria
                <ul>
                  <li>Have at least one uppercase character</li>
                  <li>Have at least one lowercase character</li>
                  <li>Have at least one special character</li>
                  <li>Have at least 12 characters</li>
                </ul>
              </Typography>
              <TextField
                required
                fullWidth
                id="validation"
                name="validation"
                label="Repeat new password"
                type="password"
                variant="standard"
                error={errors.validation !== undefined && errors.validation !== ''}
                helperText={errors.validation}
                disabled={processing}
                onBlur={handleChange}
              />
              <Box
                component="div"
                sx={{
                  py: 2
                }}
              >
                <ReCAPTCHAElement
                  ref={captchaRef}
                  sitekey={process.env.REACT_APP_SITE_KEY}
                  theme={theme.palette.mode}
                />
              </Box>
              <Button
                variant='contained'
                disabled={processing}
                onClick={handleSubmit}
              >
                Create
              </Button>
            </Box>
          </Stack>
        </form>
      </Paper>
    </Container>
  );
}

export default RecoverPassword;