self.onmessage = function (event) {
    let data = event.data;
    let result = {"data": [], "count": 0, "err": false, "msg": ""};
	
	let filtered = [];
	let uidSet = new Set();
	
	let replySet = new Set();
	if (data.deDuplicatedReply){
		data.data.sort(function(x, y){
			return x.pubTime - y.pubTime;
		});
	}
	
	for (let i = 0; i < data.data.length; ++i) {
		if (data.deDuplicatedReply){
			if (replySet.has(data.data[i].content)){
				continue;
			} else {
				replySet.add(data.data[i].content);
			}
		}
		
		if (data.onlySpecified && !data.data[i].content.includes(data.contentSpecified)){
			continue;
		}
		
		let pubTime = new Date(data.data[i].pubTime);
		if (data.limitTime && (pubTime < data.startDateTime || pubTime > data.endDateTime)){
			continue;
		}
		
		if (!data.levels.has(data.data[i].level)){
			continue;
		}
		
		if (!data.duplicatedUID){
			if (uidSet.has(data.data[i].uid)){
				continue;
			} else {
				uidSet.add(data.data[i].uid);
			}
		}
		
		filtered.push(data.data[i]);
	}
	
	if (filtered.length < data.count){
		result.err = true;
		result.msg = "符合条件的评论少于中奖人数";
		this.postMessage(result);
	}
	
	result.count = data.count;
	
	for (let i = filtered.length; i > filtered.length - data.count; --i){
		let rnd = Math.floor(Math.random() * i);
		result.data.push(filtered[rnd]);
		filtered[rnd] = filtered[i - 1];
	}
	
    this.postMessage(result);
};