import { Button, Checkbox, Container, Divider, FormControlLabel, Grid, Paper, TextField, Typography } from '@mui/material';
import { ChangeEvent, FC, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createGame } from './store/Game';
import { useAppDispatch, useAppSelector } from './store/Hooks';
import { changeWorldSettings, selectAndFocusCountry } from './store/World';

const CreateGame: FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
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
    if (game.token) {
      navigate(game.token);
		}
	}, [game])

  const handleNameChange = (event: ChangeEvent<HTMLInputElement>) => {
    setName(event.target.value);
  }

  const handleDescriptionChange = (event: ChangeEvent<HTMLInputElement>) => {
    setDescription(event.target.value);
  }

  const handleCreateGame = () => {
    dispatch(createGame({
      name,
      description
    }))
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
                value={name}
                onChange={handleNameChange}
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
                value={description}
                onChange={handleDescriptionChange}
              />
              <FormControlLabel
                control={
                  <Checkbox
                    name="tos" />}
                label="Terms of Service, bladibladibla" />
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