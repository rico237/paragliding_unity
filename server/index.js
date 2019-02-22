let app = require('express')();
let http = require('http').Server(app);
let Wemo = require('wemo-client');
let wemo = new Wemo();
let fs = require('fs');
let io = require('socket.io')(http, {
	// transports: ['websocket'],
});

// Status : 0 = OFF, 1 = ON, 8 = WAITING, -1 = NOT FOUND
const wemo_status = {
	OFF : 0, 
	ON : 1, 
	WAITING : 8, 
	NOT_FOUND : -1
}
let wemo_device_info = { 
	serialNumber : "", 
	statu: wemo_status.NOT_FOUND 
};

let joycon = {
	type: "",				// Left || Right
	gyro: "",				// x, y, z
	gyro_magnitude: "0",	
	accel: "",				// x, y, z
	accel_magnitude: "0",
	orientation: "",	 	// x, y, z, w
	rotation: "",			// x, y, z
	isUp: ""
};
let joyconL = null;
let joyconR = null;
var client;

app.get('/', function(req, res){
	fs.readFile(__dirname + '/index.html', (err, data) => {
		if (err) return res.status(500).end('Error loading index.html');
		else res.status(200).end(data);
	});
});

function connectToWemo(){
	wemo.discover( (err, deviceInfo) => {	

		if (deviceInfo.serialNumber === "221736K1201070") {
			// Get the client for the found device
			client = wemo.client(deviceInfo);
			
			wemo_device_info.serialNumber = deviceInfo.serialNumber;

			// You definitely want to listen to error events (e.g. device went offline), Node will throw them as an exception if they are left unhandled  
			client.on('error', function(err) {
				console.log('Error: %s', err.code);
			});

			// Handle BinaryState events
			client.on('binaryState', function(value) {
				console.log('Binary State changed to: %s', value);





				// TODO: tester ça !!!
				wemo_device_info.statu = value;





			});

		} 
		else console.log('Wemo Device Found: %j', deviceInfo);
		
	});
}

io.on('connection', (socket) => {
	console.log("SocketIO Ready")

	socket.on("USER_CONNECT", (data) => {
		console.log("Connected device "+ data.name);

		io.emit("USER_CONNECTED", { name: data.name });
	});	

	socket.on("JOYCON_UPDATE_LEFT", (data) => {
		// console.log(data);
		joycon = {
			type: data.type,				// Left || Right
			gyro: data.gyro,				// x, y, z
			gyro_magnitude: data.gyro_magnitude,	
			accel: data.accel,				// x, y, z
			accel_magnitude: data.accel_magnitude,
			orientation: data.orientation,	 	// x, y, z, w
			rotation: data.rotation,			// x, y, z
			pushed_buttons: data.pushed_buttons, // SHOULDER_1 || SHOULDER_2
			isUp: data.isUp
		};


		joyconL = joycon;
		io.emit("JOYCON_UPDATE_LEFT", joyconL);	
	});

	socket.on("JOYCON_UPDATE_RIGHT", (data) => {
		// console.log(data);
		joycon = {
			type: data.type,				// Left || Right
			gyro: data.gyro,				// x, y, z
			gyro_magnitude: data.gyro_magnitude,	
			accel: data.accel,				// x, y, z
			accel_magnitude: data.accel_magnitude,
			orientation: data.orientation,	 	// x, y, z, w
			rotation: data.rotation,			// x, y, z
			pushed_buttons: data.pushed_buttons, // SHOULDER_1 || SHOULDER_2
			isUp: data.isUp
		};

		joyconR = joycon;	
		io.emit("JOYCON_UPDATE_RIGHT", joyconR);
	});

	socket.on('switch_wemo', () => {
		// Turn the switch on
		if (client != undefined || client != null) {
			client.getBinaryState((err, state) => {
				if (state == 8 || state == 1) {
					client.setBinaryState(0);
					wemo_device_info.statu = wemo_status.OFF;
				}
				else {
					client.setBinaryState(1);
					wemo_device_info.statu = wemo_status.ON;
				}
			});
		}
	})

	socket.on('connect_to_wemo', () => connectToWemo());

	socket.on('switch_wemo_on', () => {
		// Turn the switch on
		if (client != undefined || client != null) {
			client.getBinaryState((err, state) => {
				if (state == 0) {
					client.setBinaryState(1);
					wemo_device_info.statu = wemo_status.ON;
				}
			});
		}
	})

	socket.on('switch_wemo_off', () => {
		// Turn the switch on
		if (client != undefined || client != null) {
			client.getBinaryState((err, state) => {
				if (state == 8 || state == 1) {
					client.setBinaryState(0);
					wemo_device_info.statu = wemo_status.OFF;
				}
			});
		}
	})

	socket.on("UPDATE_CAMERA", (data) => {
		// console.log("\nPosition : "+JSON.stringify(data.position)+"\nRotation : " + JSON.stringify(data.rotation))
		// io.emit("UPDATE_CAMERA", data);
		socket.broadcast.emit("UPDATE_CAMERA", data);
	});

	socket.on("PLAYER_MOVE", (data) => {
		// console.log("\nPosition : "+JSON.stringify(data.position)+"\nRotation : " + JSON.stringify(data.rotation))
		socket.broadcast.emit("PLAYER_MOVE", data);
	});

	socket.on('disconnect', () => {
		socket.broadcast.emit("USER_DISCONNECTED");
		console.log('disconnected')
	});

	socket.on('WIND', () => {
		socket.broadcast.emit("WIND");
		console.log('WIND')
	});



});

let port = process.env.PORT || 8000;

var listener = http.listen(port, () => { 
	console.log('/*-------------- listening on *:'+ listener.address().port +' --------------*/'); 
});