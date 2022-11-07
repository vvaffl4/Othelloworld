import { Box, Button, Checkbox, Container, Divider, FormControlLabel, Grid, LinearProgress, Paper, TextField, Typography } from '@mui/material';
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

  const [form, setForm] = useState<FormProps>({
    name: '',
    description: ''
  });
  const [errors, setErrors] = useState<Partial<FormProps>>({
    name: '',
    description: ''
  });
  const [processing, isProcessing] = useState(false);
  const player = useAppSelector(state => state.auth.player);
  const game = useAppSelector(state => state.game);

  useEffect(() => {
    //setTimeout(() => {
    //  dispatch(changeWorldSettings());
    dispatch(selectAndFocusCountry({
      isoCode: player!.username,
      altitude: 1,
      settings: {
        interactable: false,
        orbitControl: false,
        orbitAutoRotate: false,
        countrySelect: false,
        countrySelectMaxCount: 2
      }
    }));
    //}, 1000);
  }, []);

  useEffect(() => {
    if (game.hasGame) {
      isProcessing(false);
      navigate('/play');
		}
	}, [game.hasGame])

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setForm({
      ...form,
      [event.target.name]: event.target.value
    });
  }

  const handleCreateGame = () => {
    isProcessing(true);

    dispatch(createGame(
      form,
      async (result) => {
        const jsonResult = await result.json();

        if (jsonResult.errors) { 
          const errors = Object.entries((await result.json()).errors)
            .reduce((state, [key, value]) => ({
              ...state,
              [key.toLowerCase()]: (value as string[]).join('\n')
            }), {});

          setErrors(errors);
        }

        isProcessing(false);
      }));
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
              Create a Game
            </Typography>
          </Box>
          {processing && (<LinearProgress />)}
          <Box
            component="div"
            sx={{
              p: 4
						}}
          >
            <TextField
              required
              fullWidth
              id="name"
              name="name"
              label="Game name"
              variant="standard"
              sx={{
                pb: 2
							}}
              error={errors.name !== undefined
                && errors.name != ''}
              helperText={errors.name || 'The given game name can contain letters, numbers and spaces'}
              disabled={processing}
              onChange={handleChange}
            />
       {/*     <Typography*/}
       {/*       component="div"*/}
       {/*       variant="caption"*/}
       {/*       sx={{*/}
       {/*         pb: 2*/}
							{/*}}*/}
       {/*     >*/}
       {/*       The given game name can contain letters, numbers and spaces*/}
       {/*     </Typography>*/}
            <TextField
              required
              fullWidth
              id="description"
              name="description"
              label="Description"
              variant="standard"
              multiline
              rows={5}
              sx={{
                pb: 2
              }}
              error={errors.description !== undefined
                && errors.description != ''}
              helperText={errors.description || 'The given game description can\'t be longer than 400 characters.'}
              disabled={processing}
              onChange={handleChange}
            />
            {/*<Typography*/}
            {/*  component="div"*/}
            {/*  variant="caption"*/}
            {/*  sx={{*/}
            {/*    pb: 2*/}
            {/*  }}*/}
            {/*>*/}
            {/*  The given game description can't be longer than 400 characters.*/}
            {/*</Typography>*/}
            <Button
              variant='contained'
              disabled={processing}
              onClick={handleCreateGame}
            >
              Create
            </Button>
          </Box>
        </form>
      </Paper>
    </Container>
	);
};

export default CreateGame;