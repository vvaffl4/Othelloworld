import { Button, Checkbox, Container, Divider, FormControlLabel, Grid, Paper, TextField, Typography } from '@mui/material';
import { ChangeEvent, FC, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createGame } from '../store/Game';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { changeWorldSettings, selectAndFocusCountry } from '../store/World';

interface FormProps {
  name: string;
  description: string;
}

const CreateGame: FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [form, setForm] = useState<FormProps>({
    name: '',
    description: ''
  });
  const [errors, setErrors] = useState<Partial<FormProps>>({
    name: '',
    description: ''
  });
  const [isLoading, setLoading] = useState(false);
  const game = useAppSelector(state => state.game);
  //const player = useAppSelector(state => state.)

  useEffect(() => {
    dispatch(changeWorldSettings({
      interactable: false,
      orbitControl: false,
      orbitAutoRotate: false,
      countrySelect: false,
      countrySelectMaxCount: 2
    }));
    dispatch(selectAndFocusCountry({ isoCode: 'nl', altitude: 1 }));
  }, []);

  useEffect(() => {
    if (game.hasGame) {
      setLoading(false);
      navigate('/play');
		}
	}, [game])

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setForm({
      ...form,
      [event.target.name]: event.target.value
    });
  }

  const handleCreateGame = () => {
    dispatch(createGame(
      form,
      async (result) => {
        const errors = Object.entries((await result.json()).errors)
          .reduce((state, [key, value]) => ({
            ...state,
            [key.toLowerCase()]: (value as string[]).join('\n')
          }), {});

        setErrors(errors);
      }));

    setLoading(true);
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
        overflow: 'hidden'
      }}>
        <form>
          <Grid container>
            <Grid
              item
              xs={6}
              sx={{
                p: 4
              }}>
              <Typography
                variant="h3"
                color='white'
              >
                Create Game
              </Typography>
              <Divider
                sx={{
                  mb: 2
                }}
              />
              <TextField
                required
                fullWidth
                id="name"
                name="name"
                label="Game name"
                variant="standard"
                error={errors.name !== undefined
                  && errors.name != ''}
                helperText={errors.name}
                onChange={handleChange}
              />
              <TextField
                required
                fullWidth
                id="description"
                name="description"
                label="Description"
                variant="standard"
                multiline
                rows={5}
                error={errors.description !== undefined
                  && errors.description != ''}
                helperText={errors.description}
                onChange={handleChange}
              />
              <Button
                variant='contained'
                onClick={handleCreateGame}
              >
                Create
              </Button>
            </Grid>
            <Grid
              item
              xs={6}
              sx={{
                p: 4,
              }}
            >
              <TextField
                required
                id="standard-required"
                name="player_name"
                label="Required"
                variant="standard"
              />
            </Grid>
          </Grid>
        </form>
      </Paper>
    </Container>
	);
};

export default CreateGame;