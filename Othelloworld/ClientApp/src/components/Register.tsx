import { Box, Button, Checkbox, Container, Divider, FormControlLabel, Link, Paper, Stack, TextField, Typography, useTheme } from "@mui/material";
import { FC, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { register } from "../store/Auth";
import { useAppDispatch, useAppSelector } from "../store/Hooks";
import { changeWorldSettings, Country, selectCountry } from "../store/World";
import * as React from 'react';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import { FixedSizeList, ListChildComponentProps } from 'react-window';
import { shallowEqual } from "react-redux";
import ReCAPTCHAElement, { ReCAPTCHA } from "react-google-recaptcha"

function renderRow(props: ListChildComponentProps) {
  const dispatch = useAppDispatch();

  const { index, style } = props;
  const country = useAppSelector(state =>
    state.world.countries[index],
    shallowEqual);

  const handleCountryClick = () => {
    dispatch(selectCountry({ isoCode: country.properties.ISO_A2 }))
	}

  return (
    <ListItem
      style={style}
      key={index}
      component="div"
      disablePadding
      onClick={handleCountryClick}
    >
      <ListItemButton>
        <ListItemText primary={country.properties.NAME} />
      </ListItemButton>
    </ListItem>
  );
}

interface FormProps {
  username: string;
  email: string;
  password: string;
  validation: string;
  country: string
}

const Register: FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const countries = useAppSelector(state =>
    state.world.countries.map(country =>
      Object.create(country) as Country),
    shallowEqual);
  const country = useAppSelector(state => state.world.selected[0])

  const captchaRef = useRef<ReCAPTCHA>(!null)
  const [processing, isProcessing] = useState(false);
  const auth = useAppSelector(state => state.auth);
  const [form, setForm] = useState<FormProps>({
    username: '',
    email: '',
    password: '',
    validation: '',
    country: ''
  });
  const [errors, setErrors] = useState<Partial<FormProps>>({
    username: '',
    email: '',
    password: '',
    validation: '',
    country: ''
  });

  useEffect(() => {
    dispatch(changeWorldSettings({
      interactable: true,
      orbitControl: true,
      orbitAutoRotate: false,
      countrySelect: true,
      countrySelectMaxCount: 1
    }));

    return () => {
      dispatch(changeWorldSettings({
        interactable: false,
        orbitControl: false,
        orbitAutoRotate: true,
        countrySelect: false,
        countrySelectMaxCount: 0
      }));
    }
  }, [])

  useEffect(() => {
    if (auth?.authenticated !== undefined) {
      navigate(`/profile/${auth.player!.username}`, { replace: true });
    }
  }, [auth.authenticated]);

  useEffect(() => {
    if (!processing && country) {
      setForm({ ...form, country: country.properties.NAME });
    }
  }, [processing, country]);

  const handleChange = (event: React.FocusEvent<HTMLInputElement>) => {
    console.log(event.target.value);

    setForm({
      ...form,
      [event.target.name]: event.target.value
    });
  } 

  const handleSubmit = () => {
    if (form.password !== form.validation) {
      setErrors({ ...errors, validation: "Validation password does not match password" });
      return;
    }

    const countryCode = countries.find(country => country.properties.NAME === form.country)?.properties.ISO_A2;

    if (countryCode === undefined) {
      setErrors({ ...errors, country: "Country is invalid" });
      return;
		}

    isProcessing(true);

    const token = captchaRef.current.getValue();

    dispatch(register(
      { ...form, country: countryCode },
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
              Register
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
						} }
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
                id="username"
                name="username"
                label="Player name"
                variant="standard"
                error={errors.username !== undefined && errors.username !== ''}
                helperText={errors.username}
                disabled={processing}
                onBlur={handleChange}
              />
              <Typography variant="caption">
                The given username can only a-z, 0-9, or a lower dash, and must have a length between 3 - 12 characters.
              </Typography>
              <TextField
                required
                fullWidth
                id="email"
                name="email"
                label="E-mail"
                type="email"
                variant="standard"
                error={errors.email !== undefined && errors.email !== ''}
                helperText={errors.email}
                disabled={processing}
                onBlur={handleChange}
              />
              <Typography variant="caption">
                The given e-mail must be an official e-mail that can be reached.
              </Typography>
              <TextField
                required
                fullWidth
                id="password"
                name="password"
                label="Password"
                type="password"
                variant="standard"
                error={errors.password !== undefined && errors.password !== ''}
                helperText={errors.password}
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
                label="Repeat Password"
                type="password"
                variant="standard"
                error={errors.validation !== undefined && errors.validation !== ''}
                helperText={errors.validation}
                disabled={processing}
                onBlur={handleChange}
              />
              <FormControlLabel
                control={
                  <Checkbox
                    name="tos"
                    disabled={processing}
                  />
                }
                label={
                  <Typography variant="caption">
                    By checking this checkbox, you've agreed with our <Link href="#">Terms of Service</Link>.
                  </Typography>
                }
              />
              <ReCAPTCHAElement
                ref={captchaRef}
                sitekey={process.env.REACT_APP_SITE_KEY}
              />
              <Button
                variant='contained'
                disabled={processing}
                onClick={handleSubmit}
              >
                Create
              </Button>
            </Box>
            <Box
              component="div"
              style={{
                flex: 1
              }}
            >
              <TextField
                required
                id="standard-required"
                name="country"
                label="Country"
                variant="standard"
                error={errors.country !== undefined && errors.country !== ''}
                helperText={errors.country}
                fullWidth
                value={country
                  ? country.properties.NAME
                  : ''}
                disabled={processing}
                onChange={handleChange}
              />
              <Divider />
              <FixedSizeList
                height={400}
                width={'100%'}
                itemSize={46}
                itemCount={countries.length}
                overscanCount={10}
              >
                {renderRow}
              </FixedSizeList>
            </Box>
          </Stack>
        </form>
      </Paper>
    </Container>
  );
}

export default Register;