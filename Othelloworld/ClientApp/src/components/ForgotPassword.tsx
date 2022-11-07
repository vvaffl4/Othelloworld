import { Box, Button, Checkbox, Container, Divider, FormControlLabel, Link, Paper, Stack, TextField, Typography, useTheme } from "@mui/material";
import { FC, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { changePassword, forgotPassword } from "../store/Auth";
import { useAppDispatch, useAppSelector } from "../store/Hooks";
import * as React from 'react';
import { shallowEqual } from "react-redux";
import ReCAPTCHAElement, { ReCAPTCHA } from "react-google-recaptcha"

interface FormProps {
  email: string;
}

const ForgotPassword: FC = () => {
  const theme = useTheme();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const captchaRef = useRef<ReCAPTCHA>(!null)
  const [processing, isProcessing] = useState(false);

  const [form, setForm] = useState<FormProps>({
    email: ''
  });
  const [errors, setErrors] = useState<Partial<FormProps>>({
    email: ''
  });

  useEffect(() => {
    if (processing) {
      isProcessing(false);
      navigate(-1);
    }
  }, []);

  const handleChange = (event: React.FocusEvent<HTMLInputElement>) => {
    console.log(event.target.value);

    setForm({
      ...form,
      [event.target.name]: event.target.value
    });
  }

  const handleSubmit = () => {
    isProcessing(true);

    const token = captchaRef.current.getValue();

    dispatch(forgotPassword(
      form,
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
              Forgot Password
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
                id="email"
                name="email"
                label="Your e-mail"
                type="email"
                variant="standard"
                error={errors.email !== undefined && errors.email !== ''}
                helperText={errors.email}
                disabled={processing}
                onBlur={handleChange}
              />
              <Typography variant="caption">
                Enter the e-mail of your account.
              </Typography>
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

export default ForgotPassword;