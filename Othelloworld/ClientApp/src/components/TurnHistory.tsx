import { Box, ListItem, ListItemButton, ListItemIcon, ListItemText, Tab, Tabs } from '@mui/material';
import { FC, useEffect, useRef, useState } from 'react';
import { FixedSizeList, ListChildComponentProps, VariableSizeList } from 'react-window';
import { useAppSelector } from '../store/Hooks';
import HorizontalSplitIcon from '@mui/icons-material/HorizontalSplit';
import ChatOutlinedIcon from '@mui/icons-material/ChatOutlined';
import HistoryIcon from '@mui/icons-material/History';
import CircleIcon from '@mui/icons-material/Circle';
import CircleOutlinedIcon from '@mui/icons-material/CircleOutlined';
import Turn from '../model/Turn';
import Message from '../model/Message';

const TurnHistoryRow = (tab: string) => {

	return (props: ListChildComponentProps) => {
		const { index, style } = props;
		const historyItem = useAppSelector(state => state.game.history[index]);
		const type = historyItem.type
		if (type === 'history' && tab !== 'chat') {
			const item = historyItem.item as Turn;

			return (
				<ListItem
					style={style}
					key={index}
					component="div"
					disablePadding
				>
					<ListItemButton>
						<ListItemIcon>
							{item.color === 1
								? (<CircleIcon />)
								: (<CircleOutlinedIcon />)
							}
						</ListItemIcon>
						<ListItemText
							color="primary"
							primary={`Turn ${item.number}: (${item.x} ${item.y})`}
							secondary={`${item.color === 1 ? 'White' : 'Black'} player has taken X amount of stones`}
						/>
					</ListItemButton>
				</ListItem>
			)
		} else if (tab !== 'history') {
			const item = historyItem.item as Message;

			return (
				<ListItem
					style={style}
					key={index}
					component="div"
					disablePadding
				>
					<ListItemButton>
						<ListItemIcon>

						</ListItemIcon>
						<ListItemText
							color="primary"
							primary={item.username}
							secondary={item.text}
						/>
					</ListItemButton>
				</ListItem>
			);
		}
		return (
			<ListItem sx={{ display: 'none' }} key={index} component="div" disablePadding />
		);
	}
}

const TurnHistory = () => {
	const containerRef = useRef<HTMLDivElement>();
	const listRef = useRef<VariableSizeList>(null!);

	const [height, setHeight] = useState(0);
	const [tab, setTab] = useState('all');

	const history = useAppSelector(state => state.game.history);

	useEffect(() => {
		if (containerRef.current) {
			const container = containerRef.current;
			setHeight(container.clientHeight);

			const resize = () => {
				setHeight(container.clientHeight);
			}

			window.addEventListener('resize', resize);

			return () => {
				window.removeEventListener('resize', resize);
			}
		}
	}, [containerRef.current]);

	useEffect(() => {
		const list = listRef.current!;

		list.scrollToItem(history.length - 1, 'end');
	}, [history.length]);

	useEffect(() => {
		const list = listRef.current!;

		list.resetAfterIndex(0, true);
	}, [tab])

	const handleTabChange = (event: React.SyntheticEvent, newTab: string) => {
		setTab(newTab);
	};

	return (
		<Box
			component="div"
			sx={{
				display: 'flex',
				flexDirection: 'column',
				justifyContent: 'flex-end',
				height: '100%'
			}}
		>
			<Box
				ref={containerRef}
				component="div"
				sx={{
					flexGrow: 1,
					width: '100%'
				}}
			>
				<VariableSizeList
					ref={listRef}
					className='no-scrollbar'
					height={height}
					width={'100%'}
					estimatedItemSize={80}
					itemSize={(index) => {
						console.log(tab);

						return history[index].type === tab || tab === 'all' ? 80 : 0;
					}}
					itemCount={history.length}
					overscanCount={5}
				>
					{TurnHistoryRow(tab)}
				</VariableSizeList>
			</Box>
			<Box
				component="div"
				sx={{
					flexGrow: 0
				}}
			>
				<Tabs
					value={tab}
					onChange={handleTabChange}
					aria-label="icon position tabs example"
				>
					<Tab icon={<HorizontalSplitIcon />} iconPosition="start" value="all" label="All" />
					<Tab icon={<ChatOutlinedIcon />} iconPosition="start" value="chat" label="Chat" />
					<Tab icon={<HistoryIcon />} iconPosition="start" value="history" label="History" />
				</Tabs>
			</Box>
		</Box>
	);
}

export default TurnHistory;