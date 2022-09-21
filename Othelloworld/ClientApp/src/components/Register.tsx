import { Box, Button, Checkbox, Container, Divider, FormControlLabel, Paper, Stack, TextField, Typography, useTheme } from "@mui/material";
import { FC, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { register } from "../store/Auth";
import { useAppDispatch, useAppSelector } from "../store/Hooks";
import { changeWorldSettings } from "../store/World";

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

  const country = useAppSelector(state => state.world.selected[0])
  const auth = useAppSelector(state => state.auth);
  const [form, setForm] = useState<FormProps>({
    username: '',
    email: '',
    password: '',
    validation: '',
    country: ''
  });
  const [errors, setErrors] = useState<string[]>([]);

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
    console.log("auth.username: ", auth?.username);

    if (auth?.username !== undefined) {
      navigate(`/profile/${auth.username}`, { replace: true });
    }
  }, [auth]);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setForm({
      ...form,
      [event.target.name]: event.target.value
    });
  } 

  const handleSubmit = () => {
    const newErrors: string[] = [];

    if (form.password === form.validation) { 
      dispatch(register(
        form.username,
        form.email,
        form.password,
        form.country
      ));
    } else {
      newErrors.push('validation');
    }

    setErrors(newErrors);
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
        padding: 2,
        overflow: 'hidden',
        backgroundColor: 'neutral'
      }}>
        <form>
          <Stack
            direction="row"
            divider={<Divider
              orientation="vertical"
              variant="middle"
              flexItem
            />}
            spacing={2}
          >
            <Box
              component="div"
              style={{
                flex: 1
              }}
            >
              <Typography
                variant="h3"
                color='white'
              >
                Register
              </Typography>
              <Divider
                sx={{
                  mb: 2
                }}
              />
              <TextField
                required
                fullWidth
                id="username"
                name="username"
                label="Player name"
                value={form.username}
                variant="standard"
                onChange={handleChange}
              />
              <TextField
                required
                fullWidth
                id="email"
                name="email"
                label="E-mail"
                type="email"
                value={form.email}
                variant="standard"
                onChange={handleChange}
              />
              <TextField
                required
                fullWidth
                id="password"
                name="password"
                label="Password"
                type="password"
                value={form.password}
                variant="standard"
                onChange={handleChange}
              />
              <TextField
                required
                error={errors.includes('validation')}
                fullWidth
                id="validation"
                name="validation"
                label="Repeat Password"
                type="password"
                value={form.validation}
                variant="standard"
                onChange={handleChange}
              />
              <FormControlLabel
                control={
                  <Checkbox
                    name="tos" />}
                label="Terms of Service, bladibladibla" />
              <Button
                variant='contained'
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
                label="Required"
                variant="standard"
                value={country
                  ? country.properties.NAME
                  : ''}
                onChange={handleChange}
              />
            </Box>
          </Stack>
        </form>
      </Paper>
    </Container>
  );
}

export default Register;