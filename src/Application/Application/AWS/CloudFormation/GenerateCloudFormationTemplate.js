var getCidrs = function(port) {
	if (port.External) {
		return ["0.0.0.0/0"];
	} else {
		return [
			"10.0.0.0/8",
			"172.16.0.0/12",
			"192.168.0.0/16"
		];
	}
};

var getProtocols = function(port) {
	var protocols = [];
	if (port.Tcp) {
		protocols.push("tcp");
	}
	if (port.Udp) {
		protocols.push("udp");
	}

	return protocols;
};

var getUserData = function(instance, instanceIndex, stackConfiguration, platform, hostName) {
	if (platform !== "windows") {
		return getLinuxUserData(instance, stackConfiguration, hostName);
	} else {
		return getWindowsUserData(instance);
	}
};

var getLinuxUserData = function(instance, stackConfiguration, hostName) {
	var bootstrapCommand = "./bootstrap " +
		"-e FACTER_ICA_PUPPET_ROLE=" + instance.Role.Name + " " +
		"-e FACTER_ICA_PUPPET_VERSION=" + instance.VersionName + " " +
		"-e FACTER_ICA_PUPPET_ENV=" + "QA" + " " +
		"-e FACTER_ICA_PUPPET_INSTANCE_NAME=" + instance.Name + " " +
		"-i " + hostName + " " +
		"-m " + stackConfiguration.PuppetHost +
		" > /var/log/bootstrap.log 2>&1\n";

	var scriptArr = [
		"#!/bin/bash -ex\n\n"
	];

	Object.keys(hostPairs).forEach(function (key) {
		var command = "echo '" + hostPairs[key] + " " + key + "' >> /etc/hosts\n";
		scriptArr.push(command);
	});

	var newScriptArr = scriptArr.concat([
		"cd /root\n",
		"curl " + stackConfiguration.BootstrapperUrl + " > bootstrap\n",
		"chmod 700 bootstrap\n",
		bootstrapCommand,
		"curl -X PUT -H 'Content-Type:' --data-binary ",
		"'{\"Status\" : \"SUCCESS\",",
		"\"Reason\" : \"The server is ready\",",
		"\"UniqueId\" : \"0001\",",
		"\"Data\" : \"Done\"}' ",
		"\"", {
			"Ref": instance.WaitHandleName
		}, "\"\n"
	]);

	return {
		"Fn::Base64": {
			"Fn::Join": [
				"",
				newScriptArr
			]
		}
	};
};

var getWindowsUserData = function (instance) {

	var scriptArr = [
		"<script>\n"
	];

	var newlineCommand = "echo. >> C:\\Windows\\System32\\drivers\\etc\\hosts \n";
	Object.keys(hostPairs).forEach(function(key) {
		var command = "echo " + hostPairs[key] + " " + key + " >> C:\\Windows\\System32\\drivers\\etc\\hosts \n";
		scriptArr.push(newlineCommand);
		scriptArr.push(command);
	});

	var newScriptArr = scriptArr.concat([
		"cfn-init.exe -v -s ",
		{
			"Ref": "AWS::StackId"
		},
		" -r " + instance.Name,
		" --region ",
		{
			"Ref": "AWS::Region"
		},
		"\n",
		"cfn-signal.exe -e %ERRORLEVEL% ",
		{
			"Fn::Base64":
			{
				"Ref": instance.WaitHandleName
			}
		},
		"\n",
		"</script>"
	]);

	return {
		"Fn::Base64": {
			"Fn::Join": [
				"",
				newScriptArr
			]
		}
	};
};

var getWindowsMetadata = function(instance, stackConfiguration, hostName) {
	var domainName = stackConfiguration.StackName + "." + stackConfiguration.HostedZone.replace(/\.$/, "");

	var metadata = {
		"AWS::CloudFormation::Init": {
			"config": {
				"files": {
					"c:\\cfn\\cfn-hup.conf": {
						"content": {
							"Fn::Join": [
								"", [
									"[main]\n",
									"stack=", {
										"Ref": "AWS::StackId"
									}, "\n",
									"region=", {
										"Ref": "AWS::Region"
									}, "\n"
								]
							]
						}
					},
					"c:\\cfn\\hooks.d\\cfn-auto-reloader.conf": {
						"content": {
							"Fn::Join": [
								"", [
									"[cfn-auto-reloader-hook]\n",
									"triggers=post.update\n",
									"path=Resources." + instance.Name + ".Metadata.AWS::CloudFormation::Init\n",
									"action=cfn-init.exe -v -s ", {
										"Ref": "AWS::StackId"
									},
									" -r " + instance.Name,
									" --region ", {
										"Ref": "AWS::Region"
									}, "\n"
								]
							]
						}
					},
					"c:\\cfn\\software\\puppet-3.4.3.msi": {
						"source": stackConfiguration.PuppetInstallerUrl
					}
				},
				"commands": {
					"1-set_role": {
						"command": "SETX -m FACTER_ICA_PUPPET_ROLE " + instance.Role.Name,
						"waitAfterCompletion": "0"
					},
					"2-set_environment": {
						"command": "SETX -m FACTER_ICA_PUPPET_ENV QA",
						"waitAfterCompletion": "0"
					},
					"3-set_version": {
						"command": "SETX -m FACTER_ICA_PUPPET_VERSION " + instance.VersionName,
						"waitAfterCompletion": "0"
					},
					"4-set_instance_name": {
						"command": "SETX -m FACTER_ICA_PUPPET_INSTANCE_NAME " + instance.Name,
						"waitAfterCompletion": "0"
					},
					"5-set_hostname": {
						"command": "Powershell.exe \"$computer = Get-WMIObject Win32_ComputerSystem; $computer.Rename('" + hostName + "'); shutdown -t 0 -r -f\"",
						"waitAfterCompletion": "forever"
					},
					"6-set_domain": {
						"command": "Powershell.exe \"$nics = Get-WMIObject Win32_NetworkAdapterConfiguration -Filter IPEnabled=TRUE; Foreach($nic in $nics) { $nic.SetDNSDomain('" + domainName + "') }\"",
						"waitAfterCompletion": "0"
					},
					"7-install": {
						"command": "C:\\Windows\\System32\\msiexec /qn /L C:\\cfn\\msiexec.log /i C:\\cfn\\software\\puppet-3.4.3.msi PUPPET_MASTER_SERVER=" + stackConfiguration.PuppetHost,
						"waitAfterCompletion": "0"
					}
				},
				"services": {
					"windows": {
						"cfn-hup": {
							"enabled": "true",
							"ensureRunning": "true",
							"files": ["c:\\cfn\\cfn-hup.conf", "c:\\cfn\\hooks.d\\cfn-auto-reloader.conf"]
						}
					}
				}

			}
		}
	};

	return metadata;
};

var getTags = function (instance) {
	var tags = [];
	instance.Tags.forEach(function(tag) {
		tags.push({
			Key: tag.Name,
			Value: tag.Value
		});
	});

	return tags;
};

var getDedupedInstanceName = function(instance, stackConfiguration) {
	return instance.Name.replace(stackConfiguration.StackName, "");
};

var getHostName = function(instanceName, stackConfiguration) {
	return instanceName + "." + stackConfiguration.StackName + "." + stackConfiguration.HostedZone.replace(/\.$/, "");
};

// Key = rr + stackName + hostedZone
// Value = NS record object
/* Ex.
{
	"Type": "AWS::Route53::RecordSet",
	"Properties": {
		"Name": nsName,
		"HostedZoneName": data.HostedZone,
		"TTL": rr.TimeToLive,
		"Type": rr.Type,
		"ResourceRecords": [domainName]
	}
} */
var nsRecords = {};


/*
  [
    {"instanceNameStackNameHostedZone" : 10.10.55.13},
    {"anotherHostName" : 10.11.12.13}
  ]
*/
var hostPairs = [];

//module.exports = function(data, callback) {
return function(data, callback) {
	var obj = {
		"AWSTemplateFormatVersion": "2010-09-09",
		"Description": "StackIt.Net Generated Stack",
		"Parameters": {},
		"Mappings": {},
		"Conditions": {},
		"Resources": {},
		"Outputs": {}
	};

	data.Instances.forEach(function(instance) {
		instance.Role.Ports.forEach(function(port) {
			hostPairs[port.Provides] = instance.PrivateAddresses[0];
		});
	});

	data.Instances.forEach(function(instance, instanceIndex) {

		var normalizedPlatform = data.BaseImages[instanceIndex].Platform.toLowerCase();
		var dedupedStackNameInstanceName = getDedupedInstanceName(instance, data);
		var hostName = getHostName(dedupedStackNameInstanceName, data);

		// Instance
		obj.Resources[instance.Name] = {};
		obj.Resources[instance.Name].Type = "AWS::EC2::Instance";

		if (normalizedPlatform === "windows") {
			obj.Resources[instance.Name].Metadata = getWindowsMetadata(instance, data, dedupedStackNameInstanceName);
		}

		obj.Resources[instance.Name].Properties = {};
		obj.Resources[instance.Name].Properties.ImageId = data.BaseImages[instanceIndex].ResourceId;
		obj.Resources[instance.Name].Properties.KeyName = "qa-admin"; // TODO: Configuration value
		obj.Resources[instance.Name].Properties.InstanceType = instance.Role.Options.InstanceType;
		obj.Resources[instance.Name].Properties.NetworkInterfaces = [];
		obj.Resources[instance.Name].Properties.NetworkInterfaces.push({
			"DeviceIndex": 0,
			"AssociatePublicIpAddress": true,
			"PrivateIpAddress": instance.PrivateAddresses[0],
			"SubnetId": data.SubnetId,
			"GroupSet": [ data.DefaultSecurityGroupId,
				{
					"Ref": instance.SecurityGroupName
				}
			]
		});
		obj.Resources[instance.Name].Properties.Tags = getTags(instance);
		obj.Resources[instance.Name].Properties.BlockDeviceMappings = [
			{
				"DeviceName": data.BaseImages[instanceIndex].RootDeviceName,
				"Ebs": {
					"VolumeType": instance.Role.Options.VolumeType == "Ssd" ? "gp2" : "standard",
					"VolumeSize": instance.Role.Options.VolumeSize,
				}
			}
		];

		obj.Resources[instance.Name].Properties.UserData = getUserData(instance, instanceIndex, data, normalizedPlatform, hostName);

		if (instance.IamRole) {
			// instance.IamRole should be a role that was already created out of band
			// The api documentation (http://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/aws-properties-ec2-instance.html)
			// says that IamInstanceProfile must be the physical ID of an instance profile, but my own testing has shown that
			// 1. using a role or instance profile ARN causes CF to say that the iamInstanceProfile is invalid and roll back the stack and
			// 2. just passing the IAM role name actually does work.
			obj.Resources[instance.Name].Properties.IamInstanceProfile = instance.IamRole;
		}

		obj.Resources[instance.Name].DependsOn = instance.SecurityGroupName;

		// Wait handle
		obj.Resources[instance.WaitHandleName] = {
			"Type": "AWS::CloudFormation::WaitConditionHandle",
			"Properties": {}
		};

		// Wait condition
		obj.Resources[instance.WaitConditionName] = {
			"DependsOn": instance.Name,
			"Type": "AWS::CloudFormation::WaitCondition",
			"Properties": {
				"Count": 1,
				"Handle": {
					"Ref": instance.WaitHandleName
				},
				"Timeout": 5300
			}
		};

		// Security group
		var ingresses = [];
		instance.Role.Ports.forEach(function(port, portIndex) {
			var cidrs = getCidrs(port);
			var protocols = getProtocols(port);
			cidrs.forEach(function(cidr) {
				protocols.forEach(function(protocol) {
					ingresses.push({
						"IpProtocol": protocol,
						"FromPort": port.PortNumber,
						"ToPort": port.PortNumber,
						"CidrIp": cidr
					});
				});
			});

			// NS Records
			if (port.ResourceRecords) {
				var domainName = instance.Name + "." + data.HostedZone.replace(/\.$/, "");
				port.ResourceRecords.forEach(function(rr) {
					var nsName = rr.Values[0] + "." + data.StackName + "." + data.HostedZone;
					if (nsRecords[nsName] != null) {
						nsRecords[nsName].Properties.ResourceRecords.push(domainName);
					} else {
						nsRecords[nsName] = {
							"Type": "AWS::Route53::RecordSet",
							"Properties": {
								"Name": nsName,
								"HostedZoneName": data.HostedZone,
								"TTL": rr.TimeToLive,
								"Type": rr.Type,
								"ResourceRecords": [domainName]
							}
						};
					}
				});
			}
		});

		obj.Resources[instance.SecurityGroupName] = {
			"Type": "AWS::EC2::SecurityGroup",
			"Properties": {
				"GroupDescription": "Security group for " + instance.Name,
				"VpcId": instance.VpcId,
				"SecurityGroupIngress": ingresses
			}
		};

		// A Record
		obj.Resources[instance.Name + "ARecord"] = {
			"Type": "AWS::Route53::RecordSet",
			"Properties": {
				"Name": instance.Name + "." + data.HostedZone,
				"HostedZoneName": data.HostedZone,
				"TTL": 3600,
				"Type": "A",
				"ResourceRecords": [{
					"Fn::GetAtt": [instance.Name, "PublicIp"]
				}]
			}
		};
	});

	Object.keys(nsRecords).forEach(function(recordKey, index) {
		obj.Resources[data.StackName + "NS" + index] = nsRecords[recordKey];
	});

	callback(null, obj);
};